Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.bytedeco.hdf5
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports Loader = org.bytedeco.javacpp.Loader
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports org.bytedeco.hdf5.global.hdf5

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.nn.modelimport.keras


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Hdf5Archive implements java.io.Closeable
	Public Class Hdf5Archive
		Implements System.IDisposable

		Public Shared ReadOnly MAX_BUFFER_SIZE_BYTES As Integer = CInt(Math.Truncate(Math.Pow(2, 28))) '256 MB

		''' <summary>
		''' HDF5 library is not thread safe - possible to crash if multiple reads etc are performed concurrently
		''' in multiple threads. This object is used for locking read etc activity using synchronized blocks
		''' </summary>
		Public Shared ReadOnly LOCK_OBJECT As New Object()

		Shared Sub New()
			Try
				' This is necessary for the call to the BytePointer constructor below. 
				Loader.load(GetType(org.bytedeco.hdf5.global.hdf5))
			Catch e As Exception
				log.error("",e)
			End Try
		End Sub

		Private file As H5File
		Private Shared dataType As New DataType(PredType.NATIVE_FLOAT())

		Public Sub New(ByVal archiveFilename As String)
			SyncLock LOCK_OBJECT
				Me.file = New H5File(archiveFilename, H5F_ACC_RDONLY())
			End SyncLock
		End Sub

		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			SyncLock Hdf5Archive.LOCK_OBJECT
				file.deallocate()
			End SyncLock
		End Sub

		Private Function openGroups(ParamArray ByVal groups() As String) As Group()
			SyncLock Hdf5Archive.LOCK_OBJECT
				Dim groupArray(groups.Length - 1) As Group
				groupArray(0) = Me.file.openGroup(groups(0))
				For i As Integer = 1 To groups.Length - 1
					groupArray(i) = groupArray(i - 1).openGroup(groups(i))
				Next i
				Return groupArray
			End SyncLock
		End Function

		Private Sub closeGroups(ByVal groupArray() As Group)
			SyncLock Hdf5Archive.LOCK_OBJECT
				For i As Integer = groupArray.Length - 1 To 0 Step -1
					groupArray(i).deallocate()
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Read data set as ND4J array from group path.
		''' </summary>
		''' <param name="datasetName"> Name of data set </param>
		''' <param name="groups">      Array of zero or more ancestor groups from root to parent. </param>
		''' <returns> INDArray of HDF5 group data </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray readDataSet(String datasetName, String... groups) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function readDataSet(ByVal datasetName As String, ParamArray ByVal groups() As String) As INDArray
			SyncLock Hdf5Archive.LOCK_OBJECT
				If groups.Length = 0 Then
					Return readDataSet(Me.file, datasetName)
				End If
				Dim groupArray() As Group = openGroups(groups)
				Dim a As INDArray = readDataSet(groupArray(groupArray.Length - 1), datasetName)
				closeGroups(groupArray)
				Return a
			End SyncLock
		End Function

		''' <summary>
		''' Read JSON-formatted string attribute from group path.
		''' </summary>
		''' <param name="attributeName"> Name of attribute </param>
		''' <param name="groups">        Array of zero or more ancestor groups from root to parent. </param>
		''' <returns> HDF5 attribute as JSON </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public String readAttributeAsJson(String attributeName, String... groups) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function readAttributeAsJson(ByVal attributeName As String, ParamArray ByVal groups() As String) As String
			SyncLock Hdf5Archive.LOCK_OBJECT
				If groups.Length = 0 Then
					Dim a As Attribute = Me.file.openAttribute(attributeName)
					Dim s As String = readAttributeAsJson(a)
					a.deallocate()
					Return s
				End If
				Dim groupArray() As Group = openGroups(groups)
				Dim a As Attribute = groupArray(groups.Length - 1).openAttribute(attributeName)
				Dim s As String = readAttributeAsJson(a)
				a.deallocate()
				closeGroups(groupArray)
				Return s
			End SyncLock
		End Function

		''' <summary>
		''' Read string attribute from group path.
		''' </summary>
		''' <param name="attributeName"> Name of attribute </param>
		''' <param name="groups">        Array of zero or more ancestor groups from root to parent. </param>
		''' <returns> HDF5 attribute as String </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public String readAttributeAsString(String attributeName, String... groups) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function readAttributeAsString(ByVal attributeName As String, ParamArray ByVal groups() As String) As String
			SyncLock Hdf5Archive.LOCK_OBJECT
				If groups.Length = 0 Then
					Dim a As Attribute = Me.file.openAttribute(attributeName)
					Dim s As String = readAttributeAsString(a)
					a.deallocate()
					Return s
				End If
				Dim groupArray() As Group = openGroups(groups)
				Dim a As Attribute = groupArray(groups.Length - 1).openAttribute(attributeName)
				Dim s As String = readAttributeAsString(a)
				a.deallocate()
				closeGroups(groupArray)
				Return s
			End SyncLock
		End Function

		''' <summary>
		''' Check whether group path contains string attribute.
		''' </summary>
		''' <param name="attributeName"> Name of attribute </param>
		''' <param name="groups">        Array of zero or more ancestor groups from root to parent. </param>
		''' <returns> Boolean indicating whether attribute exists in group path. </returns>
		Public Overridable Function hasAttribute(ByVal attributeName As String, ParamArray ByVal groups() As String) As Boolean
			SyncLock Hdf5Archive.LOCK_OBJECT
				If groups.Length = 0 Then
					Return Me.file.attrExists(attributeName)
				End If
				Dim groupArray() As Group = openGroups(groups)
				Dim b As Boolean = groupArray(groupArray.Length - 1).attrExists(attributeName)
				closeGroups(groupArray)
				Return b
			End SyncLock
		End Function

		''' <summary>
		''' Get list of data sets from group path.
		''' </summary>
		''' <param name="groups"> Array of zero or more ancestor groups from root to parent. </param>
		''' <returns> List of HDF5 data set names </returns>
		Public Overridable Function getDataSets(ParamArray ByVal groups() As String) As IList(Of String)
			SyncLock Hdf5Archive.LOCK_OBJECT
				If groups.Length = 0 Then
					Return getObjects(Me.file, H5O_TYPE_DATASET)
				End If
				Dim groupArray() As Group = openGroups(groups)
				Dim ls As IList(Of String) = getObjects(groupArray(groupArray.Length - 1), H5O_TYPE_DATASET)
				closeGroups(groupArray)
				Return ls
			End SyncLock
		End Function

		''' <summary>
		''' Get list of groups from group path.
		''' </summary>
		''' <param name="groups"> Array of zero or more ancestor groups from root to parent. </param>
		''' <returns> List of HDF5 groups </returns>
		Public Overridable Function getGroups(ParamArray ByVal groups() As String) As IList(Of String)
			SyncLock Hdf5Archive.LOCK_OBJECT
				If groups.Length = 0 Then
					Return getObjects(Me.file, H5O_TYPE_GROUP)
				End If
				Dim groupArray() As Group = openGroups(groups)
				Dim ls As IList(Of String) = getObjects(groupArray(groupArray.Length - 1), H5O_TYPE_GROUP)
				closeGroups(groupArray)
				Return ls
			End SyncLock
		End Function

		''' <summary>
		''' Read data set as ND4J array from HDF5 group.
		''' </summary>
		''' <param name="fileGroup">   HDF5 file or group </param>
		''' <param name="datasetName"> Name of data set </param>
		''' <returns> INDArray from HDF5 data set </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.nd4j.linalg.api.ndarray.INDArray readDataSet(Group fileGroup, String datasetName) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Function readDataSet(ByVal fileGroup As Group, ByVal datasetName As String) As INDArray
			SyncLock Hdf5Archive.LOCK_OBJECT
				Dim dataset As DataSet = fileGroup.openDataSet(datasetName)
				Dim space As DataSpace = dataset.getSpace()
				Dim nbDims As Integer = space.getSimpleExtentNdims()
				Dim dims(nbDims - 1) As Long
				space.getSimpleExtentDims(dims)
				Dim dataBuffer() As Single
				Dim fp As FloatPointer
				Dim j As Integer
				Dim data As INDArray
				Select Case nbDims
					Case 5 ' 3D Convolution weights
						dataBuffer = New Single(CInt(dims(0) * dims(1) * dims(2) * dims(3) * dims(4)) - 1){}
						fp = New FloatPointer(dataBuffer)
						dataset.read(fp, dataType)
						fp.get(dataBuffer)
						data = Nd4j.create(CInt(dims(0)), CInt(dims(1)), CInt(dims(2)), CInt(dims(3)), CInt(dims(4)))
						j = 0
						Dim i1 As Integer = 0
						Do While i1 < dims(0)
							Dim i2 As Integer = 0
							Do While i2 < dims(1)
								Dim i3 As Integer = 0
								Do While i3 < dims(2)
									Dim i4 As Integer = 0
									Do While i4 < dims(3)
										Dim i5 As Integer = 0
										Do While i5 < dims(4)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: data.putScalar(new int[] { i1, i2, i3, i4, i5 }, dataBuffer[j++]);
											data.putScalar(New Integer() { i1, i2, i3, i4, i5 }, dataBuffer(j))
												j += 1
											i5 += 1
										Loop
										i4 += 1
									Loop
									i3 += 1
								Loop
								i2 += 1
							Loop
							i1 += 1
						Loop
					Case 4 ' 2D Convolution weights
						dataBuffer = New Single(CInt(dims(0) * dims(1) * dims(2) * dims(3)) - 1){}
						fp = New FloatPointer(dataBuffer)
						dataset.read(fp, dataType)
						fp.get(dataBuffer)
						data = Nd4j.create(CInt(dims(0)), CInt(dims(1)), CInt(dims(2)), CInt(dims(3)))
						j = 0
						Dim i1 As Integer = 0
						Do While i1 < dims(0)
							Dim i2 As Integer = 0
							Do While i2 < dims(1)
								Dim i3 As Integer = 0
								Do While i3 < dims(2)
									Dim i4 As Integer = 0
									Do While i4 < dims(3)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: data.putScalar(i1, i2, i3, i4, dataBuffer[j++]);
										data.putScalar(i1, i2, i3, i4, dataBuffer(j))
											j += 1
										i4 += 1
									Loop
									i3 += 1
								Loop
								i2 += 1
							Loop
							i1 += 1
						Loop
					Case 3
						dataBuffer = New Single(CInt(dims(0) * dims(1) * dims(2)) - 1){}
						fp = New FloatPointer(dataBuffer)
						dataset.read(fp, dataType)
						fp.get(dataBuffer)
						data = Nd4j.create(CInt(dims(0)), CInt(dims(1)), CInt(dims(2)))
						j = 0
						Dim i1 As Integer = 0
						Do While i1 < dims(0)
							Dim i2 As Integer = 0
							Do While i2 < dims(1)
								Dim i3 As Integer = 0
								Do While i3 < dims(2)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: data.putScalar(i1, i2, i3, dataBuffer[j++]);
									data.putScalar(i1, i2, i3, dataBuffer(j))
										j += 1
									i3 += 1
								Loop
								i2 += 1
							Loop
							i1 += 1
						Loop
					Case 2 ' Dense and Recurrent weights
						dataBuffer = New Single(CInt(dims(0) * dims(1)) - 1){}
						fp = New FloatPointer(dataBuffer)
						dataset.read(fp, dataType)
						fp.get(dataBuffer)
						data = Nd4j.create(CInt(dims(0)), CInt(dims(1)))
						j = 0
						Dim i1 As Integer = 0
						Do While i1 < dims(0)
							Dim i2 As Integer = 0
							Do While i2 < dims(1)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: data.putScalar(i1, i2, dataBuffer[j++]);
								data.putScalar(i1, i2, dataBuffer(j))
									j += 1
								i2 += 1
							Loop
							i1 += 1
						Loop
					Case 1 ' Bias
						dataBuffer = New Single(CInt(dims(0)) - 1){}
						fp = New FloatPointer(dataBuffer)
						dataset.read(fp, dataType)
						fp.get(dataBuffer)
						data = Nd4j.create(CInt(dims(0)))
						j = 0
						Dim i1 As Integer = 0
						Do While i1 < dims(0)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: data.putScalar(i1, dataBuffer[j++]);
							data.putScalar(i1, dataBuffer(j))
								j += 1
							i1 += 1
						Loop
					Case Else
						Throw New UnsupportedKerasConfigurationException("Cannot import weights with rank " & nbDims)
				End Select
				space.deallocate()
				dataset.deallocate()
				Return data
			End SyncLock
		End Function

		''' <summary>
		''' Get list of objects with a given type from a file group.
		''' </summary>
		''' <param name="fileGroup"> HDF5 file or group </param>
		''' <param name="objType">   Type of object as integer </param>
		''' <returns> List of HDF5 group objects </returns>
		Private Function getObjects(ByVal fileGroup As Group, ByVal objType As Integer) As IList(Of String)
			SyncLock Hdf5Archive.LOCK_OBJECT
				Dim groups As IList(Of String) = New List(Of String)()
				Dim i As Integer = 0
				Do While i < fileGroup.getNumObjs()
					Dim objPtr As BytePointer = fileGroup.getObjnameByIdx(i)
					If fileGroup.childObjType(objPtr) = objType Then
						groups.Add(fileGroup.getObjnameByIdx(i).getString())
					End If
					i += 1
				Loop
				Return groups
			End SyncLock
		End Function

		''' <summary>
		''' Read JSON-formatted string attribute.
		''' </summary>
		''' <param name="attribute"> HDF5 attribute to read as JSON formatted string. </param>
		''' <returns> JSON formatted string from HDF5 attribute </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private String readAttributeAsJson(Attribute attribute) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Function readAttributeAsJson(ByVal attribute As Attribute) As String
			SyncLock Hdf5Archive.LOCK_OBJECT
				Dim vl As VarLenType = attribute.getVarLenType()
				Dim currBufferLength As Integer = 2048
				Dim s As String
	'             TODO: find a less hacky way to do this.
	'             * Reading variable length strings (from attributes) is a giant
	'             * pain. There does not appear to be any way to determine the
	'             * length of the string in advance, so we use a hack: choose a
	'             * buffer size and read the config. If Jackson fails to parse
	'             * it, then we must not have read the entire config. Increase
	'             * buffer and repeat.
	'             
				Do
					Dim attrBuffer(currBufferLength - 1) As SByte
					Dim attrPointer As New BytePointer(currBufferLength)
					attribute.read(vl, attrPointer)
					attrPointer.get(attrBuffer)
					s = StringHelper.NewString(attrBuffer)
					Dim mapper As New ObjectMapper()
					mapper.enable(DeserializationFeature.FAIL_ON_READING_DUP_TREE_KEY)
					Try
						mapper.readTree(s)
						Exit Do
					Catch e As IOException
						'OK - we don't know how long the buffer needs to be, so we'll try again with larger buffer
					End Try

					If currBufferLength = MAX_BUFFER_SIZE_BYTES Then
						Throw New UnsupportedKerasConfigurationException("Could not read abnormally long HDF5 attribute: size exceeds " & currBufferLength & " bytes")
					Else
						currBufferLength = CInt(Math.Min(MAX_BUFFER_SIZE_BYTES, currBufferLength * 4L))
					End If
				Loop
				vl.deallocate()
				Return s
			End SyncLock
		End Function

		''' <summary>
		''' Read attribute as string.
		''' </summary>
		''' <param name="attribute"> HDF5 attribute to read as string. </param>
		''' <returns> HDF5 attribute as string </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private String readAttributeAsString(Attribute attribute) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Function readAttributeAsString(ByVal attribute As Attribute) As String
			SyncLock Hdf5Archive.LOCK_OBJECT
				Dim vl As VarLenType = attribute.getVarLenType()
				Dim bufferSizeMult As Integer = 1
				Dim s As String = Nothing
	'             TODO: find a less hacky way to do this.
	'             * Reading variable length strings (from attributes) is a giant
	'             * pain. There does not appear to be any way to determine the
	'             * length of the string in advance, so we use a hack: choose a
	'             * buffer size and read the config, increase buffer and repeat
	'             * until the buffer ends with \u0000
	'             
				Do
					Dim attrBuffer((bufferSizeMult * 2000) - 1) As SByte
					Dim attrPointer As New BytePointer(attrBuffer)
					attribute.read(vl, attrPointer)
					attrPointer.get(attrBuffer)
					s = StringHelper.NewString(attrBuffer)

					If s.EndsWith(ChrW(&H0000).ToString(), StringComparison.Ordinal) Then
						s = s.Replace(ChrW(&H0000).ToString(), "")
						Exit Do
					End If

					bufferSizeMult += 1
					If bufferSizeMult > 1000 Then
						Throw New UnsupportedKerasConfigurationException("Could not read abnormally long HDF5 attribute")
					End If
				Loop
				vl.deallocate()
				Return s
			End SyncLock
		End Function

		''' <summary>
		''' Read string attribute from group path.
		''' </summary>
		''' <param name="attributeName"> Name of attribute </param>
		''' <param name="bufferSize">    buffer size to read </param>
		''' <returns> Fixed-length string read from HDF5 attribute name </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public String readAttributeAsFixedLengthString(String attributeName, int bufferSize) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function readAttributeAsFixedLengthString(ByVal attributeName As String, ByVal bufferSize As Integer) As String
			SyncLock Hdf5Archive.LOCK_OBJECT
				Dim a As Attribute = Me.file.openAttribute(attributeName)
				Dim s As String = readAttributeAsFixedLengthString(a, bufferSize)
				a.deallocate()
				Return s
			End SyncLock
		End Function

		''' <summary>
		''' Read attribute of fixed buffer size as string.
		''' </summary>
		''' <param name="attribute"> HDF5 attribute to read as string. </param>
		''' <returns> Fixed-length string read from HDF5 attribute </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private String readAttributeAsFixedLengthString(Attribute attribute, int bufferSize) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Function readAttributeAsFixedLengthString(ByVal attribute As Attribute, ByVal bufferSize As Integer) As String
			SyncLock Hdf5Archive.LOCK_OBJECT
				Dim vl As VarLenType = attribute.getVarLenType()
				Dim attrBuffer(bufferSize - 1) As SByte
				Dim attrPointer As New BytePointer(attrBuffer)
				attribute.read(vl, attrPointer)
				attrPointer.get(attrBuffer)
				vl.deallocate()
				Return StringHelper.NewString(attrBuffer)
			End SyncLock
		End Function
	End Class

End Namespace