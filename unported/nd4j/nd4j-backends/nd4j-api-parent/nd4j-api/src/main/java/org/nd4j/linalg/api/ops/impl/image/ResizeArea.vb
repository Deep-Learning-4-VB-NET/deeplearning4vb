Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
Namespace org.nd4j.linalg.api.ops.impl.image


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class ResizeArea extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class ResizeArea
		Inherits DynamicCustomOp

		Protected Friend alignCorners As Boolean = False
		Protected Friend height As Integer? = Nothing
		Protected Friend width As Integer? = Nothing

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ResizeArea(@NonNull SameDiff sd, @NonNull SDVariable image, int height, int width, boolean alignCorners)
		Public Sub New(ByVal sd As SameDiff, ByVal image As SDVariable, ByVal height As Integer, ByVal width As Integer, ByVal alignCorners As Boolean)
			MyBase.New(sd, image)
			Me.alignCorners = alignCorners
			Me.height = height
			Me.width = width
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ResizeArea(@NonNull INDArray x, org.nd4j.linalg.api.ndarray.INDArray z, int height, int width, boolean alignCorners)
		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal height As Integer, ByVal width As Integer, ByVal alignCorners As Boolean)
			MyBase.New(New INDArray(){x}, New INDArray(){z})
			Me.alignCorners = alignCorners
			Me.height = height
			Me.width = width
			addArgs()
		End Sub

		Public Overrides Function opName() As String
			Return "resize_area"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "ResizeArea"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)

			Dim attrC As val = attributesForNode("align_corners")
			Me.alignCorners = If(attrC IsNot Nothing, attrC.getB(), False)

			addArgs()
		End Sub

		Protected Friend Overridable Sub addArgs()
			iArguments.Clear()
			If height IsNot Nothing AndAlso width IsNot Nothing Then
				Dim size As INDArray = Nd4j.createFromArray(New Integer(){height, width})
				addInputArgument(size)
				'iArguments.add(Long.valueOf(height));
				'iArguments.add(Long.valueOf(width));
			End If
			addBArgument(alignCorners)
		End Sub

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("alignCorners") = alignCorners
			ret("height") = height
			ret("width") = width
			Return ret
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso (inputDataTypes.Count = 1 OrElse inputDataTypes.Count = 2), "Expected 1 or 2 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(DataType.FLOAT)
		End Function
	End Class


End Namespace