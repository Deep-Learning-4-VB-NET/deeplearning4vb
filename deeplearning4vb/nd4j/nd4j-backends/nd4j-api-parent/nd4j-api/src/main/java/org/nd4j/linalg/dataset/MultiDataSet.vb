Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports org.nd4j.common.primitives
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetUtil = org.nd4j.linalg.dataset.api.DataSetUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.nd4j.linalg.dataset


	Public Class MultiDataSet
		Implements org.nd4j.linalg.dataset.api.MultiDataSet

		Private Shared ReadOnly EMPTY_MASK_ARRAY_PLACEHOLDER As New ThreadLocal(Of INDArray)()

'JAVA TO VB CONVERTER NOTE: The field features was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private features_Conflict() As INDArray
'JAVA TO VB CONVERTER NOTE: The field labels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private labels_Conflict() As INDArray
'JAVA TO VB CONVERTER NOTE: The field featuresMaskArrays was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private featuresMaskArrays_Conflict() As INDArray
'JAVA TO VB CONVERTER NOTE: The field labelsMaskArrays was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private labelsMaskArrays_Conflict() As INDArray

'JAVA TO VB CONVERTER NOTE: The field exampleMetaData was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private exampleMetaData_Conflict As IList(Of Serializable)

		''' <summary>
		''' Create a new (empty) MultiDataSet object (all fields are null) </summary>
		Public Sub New()

		End Sub

		''' <summary>
		''' MultiDataSet constructor with single features/labels input, no mask arrays
		''' </summary>
		Public Sub New(ByVal features As INDArray, ByVal labels As INDArray)
			Me.New(features, labels, Nothing, Nothing)
		End Sub

		''' <summary>
		''' MultiDataSet constructor with single features/labels input, single mask arrays
		''' </summary>
		Public Sub New(ByVal features As INDArray, ByVal labels As INDArray, ByVal featuresMask As INDArray, ByVal labelsMask As INDArray)
			Me.New((If(features IsNot Nothing, New INDArray(){features}, Nothing)), (If(labels IsNot Nothing, New INDArray(){labels}, Nothing)), (If(featuresMask IsNot Nothing, New INDArray(){featuresMask}, Nothing)), (If(labelsMask IsNot Nothing, New INDArray(){labelsMask}, Nothing)))
		End Sub

		''' <summary>
		''' MultiDataSet constructor with no mask arrays
		''' </summary>
		Public Sub New(ByVal features() As INDArray, ByVal labels() As INDArray)
			Me.New(features, labels, Nothing, Nothing)
		End Sub

		''' <param name="features">           The features (inputs) to the algorithm/neural network </param>
		''' <param name="labels">             The labels (outputs) to the algorithm/neural network </param>
		''' <param name="featuresMaskArrays"> The mask arrays for the features. May be null. Typically used with variable-length time series models, etc </param>
		''' <param name="labelsMaskArrays">   The mask arrays for the labels. May be null. Typically used with variable-length time series models, etc </param>
		Public Sub New(ByVal features() As INDArray, ByVal labels() As INDArray, ByVal featuresMaskArrays() As INDArray, ByVal labelsMaskArrays() As INDArray)
			Me.New(features, labels, featuresMaskArrays, labelsMaskArrays, Nothing)
		End Sub

		''' <param name="features">           The features (inputs) to the algorithm/neural network </param>
		''' <param name="labels">             The labels (outputs) to the algorithm/neural network </param>
		''' <param name="featuresMaskArrays"> The mask arrays for the features. May be null. Typically used with variable-length time series models, etc </param>
		''' <param name="labelsMaskArrays">   The mask arrays for the labels. May be null. Typically used with variable-length time series models, etc </param>
		''' <param name="exampleMetaData">    Metadata for each example. May be null </param>
		Public Sub New(ByVal features() As INDArray, ByVal labels() As INDArray, ByVal featuresMaskArrays() As INDArray, ByVal labelsMaskArrays() As INDArray, ByVal exampleMetaData As IList(Of Serializable))
			If features IsNot Nothing AndAlso featuresMaskArrays IsNot Nothing AndAlso features.Length <> featuresMaskArrays.Length Then
				Throw New System.ArgumentException("Invalid features / features mask arrays combination: " & "features and features mask arrays must not be different lengths")
			End If
			If labels IsNot Nothing AndAlso labelsMaskArrays IsNot Nothing AndAlso labels.Length <> labelsMaskArrays.Length Then
				Throw New System.ArgumentException("Invalid labels / labels mask arrays combination: " & "labels and labels mask arrays must not be different lengths")
			End If

			Me.features_Conflict = features
			Me.labels_Conflict = labels
			Me.featuresMaskArrays_Conflict = featuresMaskArrays
			Me.labelsMaskArrays_Conflict = labelsMaskArrays
			Me.exampleMetaData_Conflict = exampleMetaData

			Nd4j.Executioner.commit()
		End Sub

		Public Overridable Property ExampleMetaData As IList(Of Serializable)
			Get
				Return exampleMetaData_Conflict
			End Get
			Set(ByVal exampleMetaData As IList(Of T1))
				Me.exampleMetaData_Conflict = CType(exampleMetaData, IList(Of Serializable))
			End Set
		End Property

		Public Overridable Function getExampleMetaData(Of T As Serializable)(ByVal metaDataType As Type(Of T)) As IList(Of T)
			Return CType(exampleMetaData_Conflict, IList(Of T))
		End Function



		Public Overridable Function numFeatureArrays() As Integer Implements org.nd4j.linalg.dataset.api.MultiDataSet.numFeatureArrays
			Return (If(features_Conflict IsNot Nothing, features_Conflict.Length, 0))
		End Function

		Public Overridable Function numLabelsArrays() As Integer Implements org.nd4j.linalg.dataset.api.MultiDataSet.numLabelsArrays
			Return (If(labels_Conflict IsNot Nothing, labels_Conflict.Length, 0))
		End Function

		Public Overridable Property Features As INDArray() Implements org.nd4j.linalg.dataset.api.MultiDataSet.getFeatures
			Get
				Return features_Conflict
			End Get
			Set(ByVal features() As INDArray)
				Me.features_Conflict = features
			End Set
		End Property

		Public Overridable Function getFeatures(ByVal index As Integer) As INDArray Implements org.nd4j.linalg.dataset.api.MultiDataSet.getFeatures
			Return features_Conflict(index)
		End Function


		Public Overridable Sub setFeatures(ByVal idx As Integer, ByVal features As INDArray) Implements org.nd4j.linalg.dataset.api.MultiDataSet.setFeatures
			Me.features_Conflict(idx) = features
		End Sub

		Public Overridable Property Labels As INDArray() Implements org.nd4j.linalg.dataset.api.MultiDataSet.getLabels
			Get
				Return labels_Conflict
			End Get
			Set(ByVal labels() As INDArray)
				Me.labels_Conflict = labels
			End Set
		End Property

		Public Overridable Function getLabels(ByVal index As Integer) As INDArray Implements org.nd4j.linalg.dataset.api.MultiDataSet.getLabels
			Return labels_Conflict(index)
		End Function


		Public Overridable Sub setLabels(ByVal idx As Integer, ByVal labels As INDArray) Implements org.nd4j.linalg.dataset.api.MultiDataSet.setLabels
			Me.labels_Conflict(idx) = labels
		End Sub

		Public Overridable Function hasMaskArrays() As Boolean Implements org.nd4j.linalg.dataset.api.MultiDataSet.hasMaskArrays
			If featuresMaskArrays_Conflict Is Nothing AndAlso labelsMaskArrays_Conflict Is Nothing Then
				Return False
			End If
			If featuresMaskArrays_Conflict IsNot Nothing Then
				For Each i As INDArray In featuresMaskArrays_Conflict
					If i IsNot Nothing Then
						Return True
					End If
				Next i
			End If
			If labelsMaskArrays_Conflict IsNot Nothing Then
				For Each i As INDArray In labelsMaskArrays_Conflict
					If i IsNot Nothing Then
						Return True
					End If
				Next i
			End If
			Return False
		End Function

		Public Overridable Property FeaturesMaskArrays As INDArray() Implements org.nd4j.linalg.dataset.api.MultiDataSet.getFeaturesMaskArrays
			Get
				Return featuresMaskArrays_Conflict
			End Get
			Set(ByVal maskArrays() As INDArray)
				Me.featuresMaskArrays_Conflict = maskArrays
			End Set
		End Property

		Public Overridable Function getFeaturesMaskArray(ByVal index As Integer) As INDArray Implements org.nd4j.linalg.dataset.api.MultiDataSet.getFeaturesMaskArray
			Return (If(featuresMaskArrays_Conflict IsNot Nothing, featuresMaskArrays_Conflict(index), Nothing))
		End Function


		Public Overridable Sub setFeaturesMaskArray(ByVal idx As Integer, ByVal maskArray As INDArray) Implements org.nd4j.linalg.dataset.api.MultiDataSet.setFeaturesMaskArray
			Me.featuresMaskArrays_Conflict(idx) = maskArray
		End Sub

		Public Overridable ReadOnly Property LabelsMaskArrays As INDArray() Implements org.nd4j.linalg.dataset.api.MultiDataSet.getLabelsMaskArrays
			Get
				Return labelsMaskArrays_Conflict
			End Get
		End Property

		Public Overridable Function getLabelsMaskArray(ByVal index As Integer) As INDArray Implements org.nd4j.linalg.dataset.api.MultiDataSet.getLabelsMaskArray
			Return (If(labelsMaskArrays_Conflict IsNot Nothing, labelsMaskArrays_Conflict(index), Nothing))
		End Function

		Public Overridable WriteOnly Property LabelsMaskArray Implements org.nd4j.linalg.dataset.api.MultiDataSet.setLabelsMaskArray As INDArray()
			Set(ByVal labelsMaskArrays() As INDArray)
				Me.labelsMaskArrays_Conflict = labelsMaskArrays
			End Set
		End Property

		Public Overridable Sub setLabelsMaskArray(ByVal idx As Integer, ByVal labelsMaskArray As INDArray) Implements org.nd4j.linalg.dataset.api.MultiDataSet.setLabelsMaskArray
			Me.labelsMaskArrays_Conflict(idx) = labelsMaskArray
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void save(OutputStream to) throws IOException
		Public Overridable Sub save(ByVal [to] As Stream) Implements org.nd4j.linalg.dataset.api.MultiDataSet.save
			Dim numFArr As Integer = (If(features_Conflict Is Nothing, 0, features_Conflict.Length))
			Dim numLArr As Integer = (If(labels_Conflict Is Nothing, 0, labels_Conflict.Length))
			Dim numFMArr As Integer = (If(featuresMaskArrays_Conflict Is Nothing, 0, featuresMaskArrays_Conflict.Length))
			Dim numLMArr As Integer = (If(labelsMaskArrays_Conflict Is Nothing, 0, labelsMaskArrays_Conflict.Length))

			Using dos As New DataOutputStream(New BufferedOutputStream([to]))
				dos.writeInt(numFArr)
				dos.writeInt(numLArr)
				dos.writeInt(numFMArr)
				dos.writeInt(numLMArr)

				saveINDArrays(features_Conflict, dos, False)
				saveINDArrays(labels_Conflict, dos, False)
				saveINDArrays(featuresMaskArrays_Conflict, dos, True)
				saveINDArrays(labelsMaskArrays_Conflict, dos, True)

				If exampleMetaData_Conflict IsNot Nothing AndAlso exampleMetaData_Conflict.Count > 0 Then
					dos.writeInt(1)
					Dim oos As New ObjectOutputStream(dos)
					oos.writeObject(exampleMetaData_Conflict)
					oos.flush()
				End If
			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void saveINDArrays(org.nd4j.linalg.api.ndarray.INDArray[] arrays, DataOutputStream dos, boolean isMask) throws IOException
		Private Sub saveINDArrays(ByVal arrays() As INDArray, ByVal dos As DataOutputStream, ByVal isMask As Boolean)
			If arrays IsNot Nothing AndAlso arrays.Length > 0 Then
				For Each fm As INDArray In arrays
					If isMask AndAlso fm Is Nothing Then
						Dim temp As INDArray = EMPTY_MASK_ARRAY_PLACEHOLDER.get()
						If temp Is Nothing Then
							EMPTY_MASK_ARRAY_PLACEHOLDER.set(Nd4j.create(New Single() {-1}))
							temp = EMPTY_MASK_ARRAY_PLACEHOLDER.get()
						End If
						fm = temp
					End If
					Nd4j.write(fm, dos)
				Next fm
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void save(File to) throws IOException
		Public Overridable Sub save(ByVal [to] As File) Implements org.nd4j.linalg.dataset.api.MultiDataSet.save
			save(New FileStream([to], FileMode.Create, FileAccess.Write))
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void load(InputStream from) throws IOException
		Public Overridable Sub load(ByVal from As Stream) Implements org.nd4j.linalg.dataset.api.MultiDataSet.load
			Using dis As New DataInputStream(New BufferedInputStream(from))
				Dim numFArr As Integer = dis.readInt()
				Dim numLArr As Integer = dis.readInt()
				Dim numFMArr As Integer = dis.readInt()
				Dim numLMArr As Integer = dis.readInt()

				features_Conflict = loadINDArrays(numFArr, dis, False)
				labels_Conflict = loadINDArrays(numLArr, dis, False)
				featuresMaskArrays_Conflict = loadINDArrays(numFMArr, dis, True)
				labelsMaskArrays_Conflict = loadINDArrays(numLMArr, dis, True)

				Dim i As Integer
				Try
					i = dis.readInt()
				Catch e As EOFException
					'OK, no metadata to read
					Return
				End Try
				If i = 1 Then
					Dim ois As New ObjectInputStream(dis)
					Try
					Me.exampleMetaData_Conflict = CType(ois.readObject(), IList(Of Serializable))
					Catch e As ClassNotFoundException
						Throw New Exception("Error reading metadata from serialized MultiDataSet")
					End Try
				End If
			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.nd4j.linalg.api.ndarray.INDArray[] loadINDArrays(int numArrays, DataInputStream dis, boolean isMask) throws IOException
		Private Function loadINDArrays(ByVal numArrays As Integer, ByVal dis As DataInputStream, ByVal isMask As Boolean) As INDArray()
			Dim result() As INDArray = Nothing
			If numArrays > 0 Then
				result = New INDArray(numArrays - 1){}
				For i As Integer = 0 To numArrays - 1
					Dim arr As INDArray = Nd4j.read(dis)
					result(i) = If(isMask AndAlso arr.Equals(EMPTY_MASK_ARRAY_PLACEHOLDER.get()), Nothing, arr)
				Next i
			End If
			Return result
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void load(File from) throws IOException
		Public Overridable Sub load(ByVal from As File) Implements org.nd4j.linalg.dataset.api.MultiDataSet.load
			load(New FileStream(from, FileMode.Open, FileAccess.Read))
		End Sub

		Public Overridable Function asList() As IList(Of org.nd4j.linalg.dataset.api.MultiDataSet)
			Dim nExamples As Long = features_Conflict(0).size(0)

			Dim list As IList(Of org.nd4j.linalg.dataset.api.MultiDataSet) = New List(Of org.nd4j.linalg.dataset.api.MultiDataSet)()

			For i As Integer = 0 To nExamples - 1
				Dim thisFeatures(features_Conflict.Length - 1) As INDArray
				Dim thisLabels(labels_Conflict.Length - 1) As INDArray
				Dim thisFeaturesMaskArray() As INDArray = (If(featuresMaskArrays_Conflict IsNot Nothing, New INDArray(featuresMaskArrays_Conflict.Length - 1){}, Nothing))
				Dim thisLabelsMaskArray() As INDArray = (If(labelsMaskArrays_Conflict IsNot Nothing, New INDArray(labelsMaskArrays_Conflict.Length - 1){}, Nothing))

				For j As Integer = 0 To features_Conflict.Length - 1
					thisFeatures(j) = getSubsetForExample(features_Conflict(j), i)
				Next j
				For j As Integer = 0 To labels_Conflict.Length - 1
					thisLabels(j) = getSubsetForExample(labels_Conflict(j), i)
				Next j
				If thisFeaturesMaskArray IsNot Nothing Then
					For j As Integer = 0 To thisFeaturesMaskArray.Length - 1
						If featuresMaskArrays_Conflict(j) Is Nothing Then
							Continue For
						End If
						thisFeaturesMaskArray(j) = getSubsetForExample(featuresMaskArrays_Conflict(j), i)
					Next j
				End If
				If thisLabelsMaskArray IsNot Nothing Then
					For j As Integer = 0 To thisLabelsMaskArray.Length - 1
						If labelsMaskArrays_Conflict(j) Is Nothing Then
							Continue For
						End If
						thisLabelsMaskArray(j) = getSubsetForExample(labelsMaskArrays_Conflict(j), i)
					Next j
				End If

				list.Add(New MultiDataSet(thisFeatures, thisLabels, thisFeaturesMaskArray, thisLabelsMaskArray))
			Next i

			Return list
		End Function


		Private Shared Function getSubsetForExample(ByVal array As INDArray, ByVal idx As Integer) As INDArray
			'Note the interval use here: normally .point(idx) would be used, but this collapses the point dimension
			' when used on arrays with rank of 3 or greater
			'So (point,all,all) on a 3d input returns a 2d output. Whereas, we want a 3d [1,x,y] output here
			Select Case array.rank()
				Case 2
					Return array.get(NDArrayIndex.interval(idx, idx, True), NDArrayIndex.all())
				Case 3
					Return array.get(NDArrayIndex.interval(idx, idx, True), NDArrayIndex.all(), NDArrayIndex.all())
				Case 4
					Return array.get(NDArrayIndex.interval(idx, idx, True), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
				Case Else
					Throw New System.InvalidOperationException("Cannot get subset for rank " & array.rank() & " array")
			End Select
		End Function

		''' <summary>
		''' Clone the dataset
		''' </summary>
		''' <returns> a clone of the dataset </returns>
		Public Overridable Function copy() As MultiDataSet Implements org.nd4j.linalg.dataset.api.MultiDataSet.copy
			Dim ret As New MultiDataSet(copy(Features), copy(Labels))
			If labelsMaskArrays_Conflict IsNot Nothing Then
				ret.LabelsMaskArray = copy(labelsMaskArrays_Conflict)
			End If
			If featuresMaskArrays_Conflict IsNot Nothing Then
				ret.FeaturesMaskArrays = copy(featuresMaskArrays_Conflict)
			End If
			Return ret
		End Function

		Private Function copy(ByVal arrays() As INDArray) As INDArray()
			Dim result(arrays.Length - 1) As INDArray
			For i As Integer = 0 To arrays.Length - 1
				result(i) = arrays(i).dup()
			Next i
			Return result
		End Function


		''' <summary>
		''' Merge a collection of MultiDataSet objects into a single MultiDataSet.
		''' Merging is done by concatenating along dimension 0 (example number in batch)
		''' Merging operation may introduce mask arrays (when necessary) for time series data that has different lengths;
		''' if mask arrays already exist, these will be merged also.
		''' </summary>
		''' <param name="toMerge"> Collection of MultiDataSet objects to merge </param>
		''' <returns> a single MultiDataSet object, containing the arrays of </returns>
		Public Shared Function merge(Of T1 As org.nd4j.linalg.dataset.api.MultiDataSet)(ByVal toMerge As ICollection(Of T1)) As MultiDataSet
			If toMerge.Count = 1 Then
				Dim mds As org.nd4j.linalg.dataset.api.MultiDataSet = toMerge.GetEnumerator().next()
				If TypeOf mds Is MultiDataSet Then
					Return DirectCast(mds, MultiDataSet)
				Else
					Return New MultiDataSet(mds.Features, mds.Labels, mds.FeaturesMaskArrays, mds.LabelsMaskArrays)
				End If
			End If

			Dim list As IList(Of org.nd4j.linalg.dataset.api.MultiDataSet)
			If TypeOf toMerge Is System.Collections.IList Then
				list = CType(toMerge, IList(Of org.nd4j.linalg.dataset.api.MultiDataSet))
			Else
				list = New List(Of org.nd4j.linalg.dataset.api.MultiDataSet)(toMerge)
			End If

			Dim nonEmpty As Integer = 0
			For Each mds As org.nd4j.linalg.dataset.api.MultiDataSet In toMerge
				If mds.Empty Then
					Continue For
				End If
				nonEmpty += 1
			Next mds

			Dim nInArrays As Integer = list(0).numFeatureArrays()
			Dim nOutArrays As Integer = list(0).numLabelsArrays()

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim features[][] As INDArray = new INDArray[nonEmpty][0]
			Dim features()() As INDArray = RectangularArrays.RectangularINDArrayArray(nonEmpty, 0)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim labels[][] As INDArray = new INDArray[nonEmpty][0]
			Dim labels()() As INDArray = RectangularArrays.RectangularINDArrayArray(nonEmpty, 0)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim featuresMasks[][] As INDArray = new INDArray[nonEmpty][0]
			Dim featuresMasks()() As INDArray = RectangularArrays.RectangularINDArrayArray(nonEmpty, 0)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim labelsMasks[][] As INDArray = new INDArray[nonEmpty][0]
			Dim labelsMasks()() As INDArray = RectangularArrays.RectangularINDArrayArray(nonEmpty, 0)

			Dim i As Integer = 0
			For Each mds As org.nd4j.linalg.dataset.api.MultiDataSet In list
				If mds.Empty Then
					Continue For
				End If

				features(i) = mds.Features
				labels(i) = mds.Labels
				featuresMasks(i) = mds.FeaturesMaskArrays
				labelsMasks(i) = mds.LabelsMaskArrays

				If features(i) Is Nothing OrElse features(i).Length <> nInArrays Then
					Throw New System.InvalidOperationException("Cannot merge MultiDataSets with different number of input arrays: toMerge[0] has " & nInArrays & " input arrays; toMerge[" & i & "] has " & (If(features(i) IsNot Nothing, features(i).Length, Nothing)) & " arrays")
				End If
				If labels(i) Is Nothing OrElse labels(i).Length <> nOutArrays Then
					Throw New System.InvalidOperationException("Cannot merge MultiDataSets with different number of output arrays: toMerge[0] has " & nOutArrays & " output arrays; toMerge[" & i & "] has " & (If(labels(i) IsNot Nothing, labels(i).Length, Nothing)) & " arrays")
				End If

				i += 1
			Next mds

			'Now, merge:
			Dim mergedFeatures(nInArrays - 1) As INDArray
			Dim mergedLabels(nOutArrays - 1) As INDArray
			Dim mergedFeaturesMasks(nInArrays - 1) As INDArray
			Dim mergedLabelsMasks(nOutArrays - 1) As INDArray

			Dim needFeaturesMasks As Boolean = False
			For i = 0 To nInArrays - 1
				Dim pair As Pair(Of INDArray, INDArray) = DataSetUtil.mergeFeatures(features, featuresMasks, i) 'merge(features, featuresMasks, i);
				mergedFeatures(i) = pair.First
				mergedFeaturesMasks(i) = pair.Second
				If mergedFeaturesMasks(i) IsNot Nothing Then
					needFeaturesMasks = True
				End If
			Next i
			If Not needFeaturesMasks Then
				mergedFeaturesMasks = Nothing
			End If

			Dim needLabelsMasks As Boolean = False
			For i = 0 To nOutArrays - 1
				Dim pair As Pair(Of INDArray, INDArray) = DataSetUtil.mergeLabels(labels, labelsMasks, i)
				mergedLabels(i) = pair.First
				mergedLabelsMasks(i) = pair.Second
				If mergedLabelsMasks(i) IsNot Nothing Then
					needLabelsMasks = True
				End If
			Next i
			If Not needLabelsMasks Then
				mergedLabelsMasks = Nothing
			End If

			Return New MultiDataSet(mergedFeatures, mergedLabels, mergedFeaturesMasks, mergedLabelsMasks)
		End Function


		Public Overrides Function ToString() As String
			Dim nfMask As Integer = 0
			Dim nlMask As Integer = 0
			If featuresMaskArrays_Conflict IsNot Nothing Then
				For Each i As INDArray In featuresMaskArrays_Conflict
					If i IsNot Nothing Then
						nfMask += 1
					End If
				Next i
			End If
			If labelsMaskArrays_Conflict IsNot Nothing Then
				For Each i As INDArray In labelsMaskArrays_Conflict
					If i IsNot Nothing Then
						nlMask += 1
					End If
				Next i
			End If

			Dim sb As New StringBuilder()
			sb.Append("MultiDataSet: ").Append(numFeatureArrays()).Append(" input arrays, ").Append(numLabelsArrays()).Append(" label arrays, ").Append(nfMask).Append(" input masks, ").Append(nlMask).Append(" label masks")


			Dim i As Integer = 0
			Do While i < numFeatureArrays()
				sb.Append(vbLf & "=== INPUT ").Append(i).Append(" ===" & vbLf).Append(getFeatures(i).ToString().replaceAll(";", vbLf))
				If getFeaturesMaskArray(i) IsNot Nothing Then
					sb.Append(vbLf & "--- INPUT MASK ---" & vbLf).Append(getFeaturesMaskArray(i).ToString().replaceAll(";", vbLf))
				End If
				i += 1
			Loop
			i = 0
			Do While i<numLabelsArrays()
				sb.Append(vbLf & "=== LABEL ").Append(i).Append(" ===" & vbLf).Append(getLabels(i).ToString().replaceAll(";", vbLf))

				If getLabelsMaskArray(i) IsNot Nothing Then
					sb.Append(vbLf & "--- LABEL MASK ---" & vbLf).Append(getLabelsMaskArray(i).ToString().replaceAll(";", vbLf))
				End If
				i += 1
			Loop
			Return sb.ToString()
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then
				Return True
			End If
			If Not (TypeOf o Is MultiDataSet) Then
				Return False
			End If

			Dim m As MultiDataSet = DirectCast(o, MultiDataSet)

			If Not bothNullOrEqual(features_Conflict, m.features_Conflict) Then
				Return False
			End If
			If Not bothNullOrEqual(labels_Conflict, m.labels_Conflict) Then
				Return False
			End If
			If Not bothNullOrEqual(featuresMaskArrays_Conflict, m.featuresMaskArrays_Conflict) Then
				Return False
			End If
			Return bothNullOrEqual(labelsMaskArrays_Conflict, m.labelsMaskArrays_Conflict)
		End Function

		Private Function bothNullOrEqual(ByVal first() As INDArray, ByVal second() As INDArray) As Boolean
			If first Is Nothing AndAlso second Is Nothing Then
				Return True
			End If
			If first Is Nothing OrElse second Is Nothing Then
				Return False 'One but not both null
			End If
			If first.Length <> second.Length Then
				Return False
			End If
			For i As Integer = 0 To first.Length - 1
				If Not Objects.equals(first(i), second(i)) Then
					Return False
				End If
			Next i
			Return True
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 0
			If features_Conflict IsNot Nothing Then
				For Each f As INDArray In features_Conflict
					result = result * 31 + f.GetHashCode()
				Next f
			End If
			If labels_Conflict IsNot Nothing Then
				For Each l As INDArray In labels_Conflict
					result = result * 31 + l.GetHashCode()
				Next l
			End If
			If featuresMaskArrays_Conflict IsNot Nothing Then
				For Each fm As INDArray In featuresMaskArrays_Conflict
					result = result * 31 + fm.GetHashCode()
				Next fm
			End If
			If labelsMaskArrays_Conflict IsNot Nothing Then
				For Each lm As INDArray In labelsMaskArrays_Conflict
					result = result * 31 + lm.GetHashCode()
				Next lm
			End If
			Return result
		End Function

		''' <summary>
		''' This method returns memory used by this DataSet
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property MemoryFootprint As Long Implements org.nd4j.linalg.dataset.api.MultiDataSet.getMemoryFootprint
			Get
				Dim reqMem As Long = 0
    
				For Each f As INDArray In features_Conflict
					reqMem += If(f Is Nothing, 0, f.length() * Nd4j.sizeOfDataType(f.dataType()))
				Next f
    
				If featuresMaskArrays_Conflict IsNot Nothing Then
					For Each f As INDArray In featuresMaskArrays_Conflict
						reqMem += If(f Is Nothing, 0, f.length() * Nd4j.sizeOfDataType(f.dataType()))
					Next f
				End If
    
				If labelsMaskArrays_Conflict IsNot Nothing Then
					For Each f As INDArray In labelsMaskArrays_Conflict
						reqMem += If(f Is Nothing, 0, f.length() * Nd4j.sizeOfDataType(f.dataType()))
					Next f
				End If
    
				If labels_Conflict IsNot Nothing Then
					For Each f As INDArray In labels_Conflict
						reqMem += If(f Is Nothing, 0, f.length() * Nd4j.sizeOfDataType(f.dataType()))
					Next f
				End If
    
				Return reqMem
			End Get
		End Property


		Public Overridable Sub migrate() Implements org.nd4j.linalg.dataset.api.MultiDataSet.migrate
			If Nd4j.MemoryManager.CurrentWorkspace IsNot Nothing Then
				If features_Conflict IsNot Nothing Then
					For e As Integer = 0 To features_Conflict.Length - 1
						features_Conflict(e) = features_Conflict(e).migrate()
					Next e
				End If

				If labels_Conflict IsNot Nothing Then
					For e As Integer = 0 To labels_Conflict.Length - 1
						labels_Conflict(e) = labels_Conflict(e).migrate()
					Next e
				End If

				If featuresMaskArrays_Conflict IsNot Nothing Then
					For e As Integer = 0 To featuresMaskArrays_Conflict.Length - 1
						featuresMaskArrays_Conflict(e) = featuresMaskArrays_Conflict(e).migrate()
					Next e
				End If

				If labelsMaskArrays_Conflict IsNot Nothing Then
					For e As Integer = 0 To labelsMaskArrays_Conflict.Length - 1
						labelsMaskArrays_Conflict(e) = labelsMaskArrays_Conflict(e).migrate()
					Next e
				End If
			End If
		End Sub

		''' <summary>
		''' This method migrates this DataSet into current Workspace (if any)
		''' </summary>
		Public Overridable Sub detach() Implements org.nd4j.linalg.dataset.api.MultiDataSet.detach
			If features_Conflict IsNot Nothing Then
				For e As Integer = 0 To features_Conflict.Length - 1
					features_Conflict(e) = features_Conflict(e).detach()
				Next e
			End If

			If labels_Conflict IsNot Nothing Then
				For e As Integer = 0 To labels_Conflict.Length - 1
					labels_Conflict(e) = labels_Conflict(e).detach()
				Next e
			End If

			If featuresMaskArrays_Conflict IsNot Nothing Then
				For e As Integer = 0 To featuresMaskArrays_Conflict.Length - 1
					featuresMaskArrays_Conflict(e) = featuresMaskArrays_Conflict(e).detach()
				Next e
			End If

			If labelsMaskArrays_Conflict IsNot Nothing Then
				For e As Integer = 0 To labelsMaskArrays_Conflict.Length - 1
					labelsMaskArrays_Conflict(e) = labelsMaskArrays_Conflict(e).detach()
				Next e
			End If
		End Sub

		Public Overridable ReadOnly Property Empty As Boolean Implements org.nd4j.linalg.dataset.api.MultiDataSet.isEmpty
			Get
				Return nullOrEmpty(features_Conflict) AndAlso nullOrEmpty(labels_Conflict) AndAlso nullOrEmpty(featuresMaskArrays_Conflict) AndAlso nullOrEmpty(labelsMaskArrays_Conflict)
			End Get
		End Property

		Public Overridable Sub shuffle() Implements org.nd4j.linalg.dataset.api.MultiDataSet.shuffle
			Dim split As IList(Of org.nd4j.linalg.dataset.api.MultiDataSet) = New List(Of org.nd4j.linalg.dataset.api.MultiDataSet) From {}
			Collections.shuffle(split)
			Dim mds As MultiDataSet = merge(split)
			Me.features_Conflict = mds.features_Conflict
			Me.labels_Conflict = mds.labels_Conflict
			Me.featuresMaskArrays_Conflict = mds.featuresMaskArrays_Conflict
			Me.labelsMaskArrays_Conflict = mds.labelsMaskArrays_Conflict
			Me.exampleMetaData_Conflict = mds.exampleMetaData_Conflict
		End Sub

		Private Shared Function nullOrEmpty(ByVal arr() As INDArray) As Boolean
			If arr Is Nothing Then
				Return True
			End If
			For Each i As INDArray In arr
				If i IsNot Nothing Then
					Return False
				End If
			Next i
			Return True
		End Function
	End Class

End Namespace