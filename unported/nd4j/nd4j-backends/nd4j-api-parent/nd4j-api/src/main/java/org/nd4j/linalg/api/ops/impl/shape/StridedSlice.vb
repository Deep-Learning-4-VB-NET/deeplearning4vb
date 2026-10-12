Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports StridedSliceBp = org.nd4j.linalg.api.ops.impl.shape.bp.StridedSliceBp
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.impl.shape


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class StridedSlice extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class StridedSlice
		Inherits DynamicCustomOp

		Private begin() As Long
		Private [end]() As Long
		Private strides() As Long
		Private beginMask As Integer
		Private endMask As Integer
		Private ellipsisMask As Integer
		Private newAxisMask As Integer
		Private shrinkAxisMask As Integer

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal begin() As Integer, ByVal [end]() As Integer, ByVal strides() As Integer)
			Me.New(sameDiff, [in], begin, [end], strides, 0, 0, 0, 0, 0)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal begin() As Long, ByVal [end]() As Long, ByVal strides() As Long)
			Me.New(sameDiff, [in], begin, [end], strides, 0, 0, 0, 0, 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StridedSlice(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable in, @NonNull long[] begin, @NonNull long[] end, @NonNull long[] strides, int beginMask, int endMask, int ellipsisMask, int newAxisMask, int shrinkAxisMask)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal begin() As Long, ByVal [end]() As Long, ByVal strides() As Long, ByVal beginMask As Integer, ByVal endMask As Integer, ByVal ellipsisMask As Integer, ByVal newAxisMask As Integer, ByVal shrinkAxisMask As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){[in]})
			Me.begin = begin
			Me.end = [end]
			Me.strides = strides
			Me.beginMask = beginMask
			Me.endMask = endMask
			Me.ellipsisMask = ellipsisMask
			Me.newAxisMask = newAxisMask
			Me.shrinkAxisMask = shrinkAxisMask

			'https://github.com/deeplearning4j/libnd4j/blob/master/include/ops/declarable/generic/parity_ops/strided_slice.cpp#L279
			addArguments()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StridedSlice(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable in, @NonNull int[] begin, @NonNull int[] end, @NonNull int[] strides, int beginMask, int endMask, int ellipsisMask, int newAxisMask, int shrinkAxisMask)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal begin() As Integer, ByVal [end]() As Integer, ByVal strides() As Integer, ByVal beginMask As Integer, ByVal endMask As Integer, ByVal ellipsisMask As Integer, ByVal newAxisMask As Integer, ByVal shrinkAxisMask As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){[in]})
			Me.begin = ArrayUtil.toLongArray(begin)
			Me.end = ArrayUtil.toLongArray([end])
			Me.strides = ArrayUtil.toLongArray(strides)
			Me.beginMask = beginMask
			Me.endMask = endMask
			Me.ellipsisMask = ellipsisMask
			Me.newAxisMask = newAxisMask
			Me.shrinkAxisMask = shrinkAxisMask
			addArguments()
			'https://github.com/deeplearning4j/libnd4j/blob/master/include/ops/declarable/generic/parity_ops/strided_slice.cpp#L279

		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal begin() As Integer, ByVal [end]() As Integer, ByVal strides() As Integer, ByVal beginMask As Integer, ByVal endMask As Integer, ByVal ellipsisMask As Integer, ByVal newAxisMask As Integer, ByVal shrinkAxisMask As Integer)
			Me.New([in], ArrayUtil.toLongArray(begin), ArrayUtil.toLongArray([end]), ArrayUtil.toLongArray(strides), beginMask, endMask, ellipsisMask, newAxisMask, shrinkAxisMask)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal begin() As Long, ByVal [end]() As Long, ByVal strides() As Long, ByVal beginMask As Integer, ByVal endMask As Integer, ByVal ellipsisMask As Integer, ByVal newAxisMask As Integer, ByVal shrinkAxisMask As Integer)
			addInputArgument([in])
			Me.begin = begin
			Me.end = [end]
			Me.strides = strides
			Me.beginMask = beginMask
			Me.endMask = endMask
			Me.ellipsisMask = ellipsisMask
			Me.newAxisMask = newAxisMask
			Me.shrinkAxisMask = shrinkAxisMask
			addArguments()
		End Sub

		Private Sub addArguments()
			addIArgument(beginMask)
			addIArgument(ellipsisMask)
			addIArgument(endMask)
			addIArgument(newAxisMask)
			addIArgument(shrinkAxisMask)
			addIArgument(begin)
			addIArgument([end])
			addIArgument(strides)
		End Sub


		Public Overrides Function opName() As String
			Return "strided_slice"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "StridedSlice"
		End Function


		Public Overrides Sub assertValidForExecution()
			If numInputArguments() <> 1 AndAlso numInputArguments() <> 3 AndAlso numInputArguments() <> 4 Then
				Throw New ND4JIllegalStateException("Num input arguments must be 1 3 or 4.")
			End If

			If numIArguments() < 5 Then
				Throw New ND4JIllegalStateException("Number of integer arguments must >= 5")
			End If
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim inputBegin As val = nodeDef.getInput(1)
			Dim inputEnd As val = nodeDef.getInput(2)
			Dim inputStrides As val = nodeDef.getInput(3)

			' bit masks for this slice
			Dim bm As val = nodeDef.getAttrOrThrow("begin_mask")
			Dim xm As val = nodeDef.getAttrOrThrow("ellipsis_mask")
			Dim em As val = nodeDef.getAttrOrThrow("end_mask")
			Dim nm As val = nodeDef.getAttrOrThrow("new_axis_mask")
			Dim sm As val = nodeDef.getAttrOrThrow("shrink_axis_mask")

			beginMask = CInt(Math.Truncate(bm.getI()))
			ellipsisMask = CInt(Math.Truncate(xm.getI()))
			endMask = CInt(Math.Truncate(em.getI()))
			newAxisMask = CInt(Math.Truncate(nm.getI()))
			shrinkAxisMask = CInt(Math.Truncate(sm.getI()))

			addIArgument(beginMask)
			addIArgument(ellipsisMask)
			addIArgument(endMask)
			addIArgument(newAxisMask)
			addIArgument(shrinkAxisMask)
		End Sub



		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim beginMapping As val = PropertyMapping.builder().tfInputPosition(1).propertyNames(New String(){"begin"}).build()

			Dim [end] As val = PropertyMapping.builder().tfInputPosition(2).propertyNames(New String(){"end"}).build()


			Dim strides As val = PropertyMapping.builder().tfInputPosition(3).propertyNames(New String(){"strides"}).build()




			Dim beginMask As val = PropertyMapping.builder().tfAttrName("begin_mask").propertyNames(New String(){"beginMask"}).build()


			Dim ellipsisMask As val = PropertyMapping.builder().tfAttrName("ellipsis_mask").propertyNames(New String(){"ellipsisMask"}).build()



			Dim endMask As val = PropertyMapping.builder().tfAttrName("end_mask").propertyNames(New String(){"endMask"}).build()



			Dim newAxisMask As val = PropertyMapping.builder().tfAttrName("new_axis_mask").propertyNames(New String(){"newAxisMask"}).build()

			Dim shrinkAxisMask As val = PropertyMapping.builder().tfAttrName("shrink_axis_mask").propertyNames(New String(){"shrinkAxisMask"}).build()



			map("begin") = beginMapping
			map("end") = [end]
			map("strides") = strides
			map("beginMask") = beginMask
			map("ellipsisMask") = ellipsisMask
			map("endMask") = endMask
			map("newAxisMask") = newAxisMask
			map("shrinkAxisMask") = shrinkAxisMask


			ret(tensorflowName()) = map

			Return ret
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			If args().Length = 1 Then
				'Array inputs for begin/end/strides
				Return (New StridedSliceBp(sameDiff, arg(), i_v(0), begin, [end], strides, beginMask, endMask, ellipsisMask, newAxisMask, shrinkAxisMask)).outputs()
			Else
				'SDVariable inputs for begin/end/strides
				Return (New StridedSliceBp(sameDiff, arg(), i_v(0), arg(1), arg(2), arg(3), beginMask, endMask, ellipsisMask, newAxisMask, shrinkAxisMask)).outputs()
			End If
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 4), "Expected 1 or 4 input datatypes for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as input type. 1 or 4 depending on whether using iargs or arrays (for TF import etc)
			Return Collections.singletonList(dataTypes(0))
		End Function

	End Class

End Namespace