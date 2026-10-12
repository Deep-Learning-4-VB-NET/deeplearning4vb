Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
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
'ORIGINAL LINE: @NoArgsConstructor public class ResizeBicubic extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class ResizeBicubic
		Inherits DynamicCustomOp

		Protected Friend alignCorners As Boolean = False
		Protected Friend alignPixelCenters As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ResizeBicubic(@NonNull INDArray image, org.nd4j.linalg.api.ndarray.INDArray size, boolean alignCorners, boolean alignPixelCenters)
		Public Sub New(ByVal image As INDArray, ByVal size As INDArray, ByVal alignCorners As Boolean, ByVal alignPixelCenters As Boolean)
			addInputArgument(image, size)
			addBArgument(alignCorners, alignPixelCenters)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ResizeBicubic(@NonNull SameDiff sameDiff, @NonNull SDVariable image, org.nd4j.autodiff.samediff.SDVariable size, boolean alignCorners, boolean alignPixelCenters)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal image As SDVariable, ByVal size As SDVariable, ByVal alignCorners As Boolean, ByVal alignPixelCenters As Boolean)
			MyBase.New(sameDiff, New SDVariable(){image, size})
			addBArgument(alignCorners, alignPixelCenters)
		End Sub

		Public Overrides Function opName() As String
			Return "resize_bicubic"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "ResizeBicubic"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)

			Me.alignCorners = attributesForNode("align_corners").getB()
			Me.alignPixelCenters = attributesForNode("half_pixel_centers").getB()
			addBArgument(alignCorners, alignPixelCenters)
		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso (inputDataTypes.Count = 1 OrElse inputDataTypes.Count = 2), "Expected 1 or 2 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(Nd4j.defaultFloatingPointType())
		End Function
	End Class

End Namespace