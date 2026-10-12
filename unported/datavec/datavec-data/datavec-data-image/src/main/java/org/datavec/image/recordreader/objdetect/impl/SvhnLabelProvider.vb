Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports IntPointer = org.bytedeco.javacpp.IntPointer
Imports Loader = org.bytedeco.javacpp.Loader
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports PointerPointer = org.bytedeco.javacpp.PointerPointer
Imports ImageObject = org.datavec.image.recordreader.objdetect.ImageObject
Imports ImageObjectLabelProvider = org.datavec.image.recordreader.objdetect.ImageObjectLabelProvider
Imports org.bytedeco.hdf5
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

Namespace org.datavec.image.recordreader.objdetect.impl


	Public Class SvhnLabelProvider
		Implements ImageObjectLabelProvider

		Private Shared refType As New DataType(PredType.STD_REF_OBJ())
		Private Shared charType As New DataType(PredType.NATIVE_CHAR())
		Private Shared intType As New DataType(PredType.NATIVE_INT())

		Private labelMap As IDictionary(Of String, IList(Of ImageObject))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public SvhnLabelProvider(java.io.File dir) throws java.io.IOException
		Public Sub New(ByVal dir As File)
			labelMap = New Dictionary(Of String, IList(Of ImageObject))()

			Dim file As New H5File(dir.getPath() & "/digitStruct.mat", H5F_ACC_RDONLY())
			Dim group As Group = file.openGroup("digitStruct")
			Dim nameDataset As DataSet = group.openDataSet("name")
			Dim [nameSpace] As DataSpace = nameDataset.getSpace()
			Dim bboxDataset As DataSet = group.openDataSet("bbox")
			Dim bboxSpace As DataSpace = bboxDataset.getSpace()
			Dim dims(1) As Long
			bboxSpace.getSimpleExtentDims(dims)
			Dim n As Integer = CInt(dims(0) * dims(1))

			Dim ptrSize As Integer = Loader.sizeof(GetType(Pointer))
			Dim namePtr As New PointerPointer(n)
			Dim bboxPtr As New PointerPointer(n)
			nameDataset.read(namePtr, refType)
			bboxDataset.read(bboxPtr, refType)

			Dim bytePtr As New BytePointer(256)
			Dim topPtr As New PointerPointer(256)
			Dim leftPtr As New PointerPointer(256)
			Dim heightPtr As New PointerPointer(256)
			Dim widthPtr As New PointerPointer(256)
			Dim labelPtr As New PointerPointer(256)
			Dim intPtr As New IntPointer(256)
			For i As Integer = 0 To n - 1
				Dim nameRef As New DataSet(file, namePtr.position(i * ptrSize))
				nameRef.read(bytePtr, charType)
				Dim filename As String = bytePtr.getString()

				Dim bboxGroup As New Group(file, bboxPtr.position(i * ptrSize))
				Dim topDataset As DataSet = bboxGroup.openDataSet("top")
				Dim leftDataset As DataSet = bboxGroup.openDataSet("left")
				Dim heightDataset As DataSet = bboxGroup.openDataSet("height")
				Dim widthDataset As DataSet = bboxGroup.openDataSet("width")
				Dim labelDataset As DataSet = bboxGroup.openDataSet("label")

				Dim topSpace As DataSpace = topDataset.getSpace()
				topSpace.getSimpleExtentDims(dims)
				Dim m As Integer = CInt(dims(0) * dims(1))
				Dim list As New List(Of ImageObject)(m)

				Dim isFloat As Boolean = topDataset.asAbstractDs().getTypeClass() = H5T_FLOAT
				If Not isFloat Then
					topDataset.read(topPtr.position(0), refType)
					leftDataset.read(leftPtr.position(0), refType)
					heightDataset.read(heightPtr.position(0), refType)
					widthDataset.read(widthPtr.position(0), refType)
					labelDataset.read(labelPtr.position(0), refType)
				End If
				Debug.Assert(Not isFloat OrElse m = 1)

				For j As Integer = 0 To m - 1
					Dim topSet As DataSet = If(isFloat, topDataset, New DataSet(file, topPtr.position(j * ptrSize)))
					topSet.read(intPtr, intType)
					Dim top As Integer = intPtr.get()

					Dim leftSet As DataSet = If(isFloat, leftDataset, New DataSet(file, leftPtr.position(j * ptrSize)))
					leftSet.read(intPtr, intType)
					Dim left As Integer = intPtr.get()

					Dim heightSet As DataSet = If(isFloat, heightDataset, New DataSet(file, heightPtr.position(j * ptrSize)))
					heightSet.read(intPtr, intType)
					Dim height As Integer = intPtr.get()

					Dim widthSet As DataSet = If(isFloat, widthDataset, New DataSet(file, widthPtr.position(j * ptrSize)))
					widthSet.read(intPtr, intType)
					Dim width As Integer = intPtr.get()

					Dim labelSet As DataSet = If(isFloat, labelDataset, New DataSet(file, labelPtr.position(j * ptrSize)))
					labelSet.read(intPtr, intType)
					Dim label As Integer = intPtr.get()
					If label = 10 Then
						label = 0
					End If

					list.Add(New ImageObject(left, top, left + width, top + height, Convert.ToString(label)))

					topSet.deallocate()
					leftSet.deallocate()
					heightSet.deallocate()
					widthSet.deallocate()
					labelSet.deallocate()
				Next j

				topSpace.deallocate()
				If Not isFloat Then
					topDataset.deallocate()
					leftDataset.deallocate()
					heightDataset.deallocate()
					widthDataset.deallocate()
					labelDataset.deallocate()
				End If
				nameRef.deallocate()
				bboxGroup.deallocate()

				labelMap(filename) = list
			Next i

			[nameSpace].deallocate()
			bboxSpace.deallocate()
			nameDataset.deallocate()
			bboxDataset.deallocate()
			group.deallocate()
			file.deallocate()
		End Sub

		Public Overridable Function getImageObjectsForPath(ByVal path As String) As IList(Of ImageObject) Implements ImageObjectLabelProvider.getImageObjectsForPath
			Dim file As New File(path)
			Dim filename As String = file.getName()
			Return labelMap(filename)
		End Function

		Public Overridable Function getImageObjectsForPath(ByVal uri As URI) As IList(Of ImageObject) Implements ImageObjectLabelProvider.getImageObjectsForPath
			Return getImageObjectsForPath(uri.ToString())
		End Function
	End Class

End Namespace