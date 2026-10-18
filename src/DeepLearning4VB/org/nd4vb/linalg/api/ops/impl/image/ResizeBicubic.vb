Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.image

    ''' <summary>The custom op <c>resize_bicubic</c> (Java <c>org.nd4j.linalg.api.ops.impl.image.ResizeBicubic</c>).</summary>
    Public Class ResizeBicubic
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend alignCorners As Boolean = False
        Protected Friend alignPixelCenters As Boolean = False

        Public Sub New(ByVal image As INDArray, ByVal size As INDArray, ByVal alignCorners As Boolean, ByVal alignPixelCenters As Boolean)
            addInputArgument(image, size)
            addBArgument(alignCorners, alignPixelCenters)
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ResizeBicubic(@NonNull SameDiff sameDiff, @NonNull SDVariable image, org.nd4j.autodiff.samediff.SDVariable size, boolean alignCorners, boolean alignPixelCenters)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal image As SDVariable, ByVal size As SDVariable, ByVal alignCorners As Boolean, ByVal alignPixelCenters As Boolean)
			MyBase.New(sameDiff, New SDVariable(){image, size})
			addBArgument(alignCorners, alignPixelCenters)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "resize_bicubic"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "ResizeBicubic"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)

			Me.alignCorners = attributesForNode("align_corners").getB()
			Me.alignPixelCenters = attributesForNode("half_pixel_centers").getB()
			addBArgument(alignCorners, alignPixelCenters)
		End Sub
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso (inputDataTypes.Count = 1 OrElse inputDataTypes.Count = 2), "Expected 1 or 2 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {Nd4vbT.defaultFloatingPointType()}
        End Function

    End Class

End Namespace
