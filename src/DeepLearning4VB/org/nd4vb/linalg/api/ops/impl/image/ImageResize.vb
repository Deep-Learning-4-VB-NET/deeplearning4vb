Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports ImageResizeMethod = DeepLearning4VB.org.nd4vb.enums.ImageResizeMethod
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
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

    ''' <summary>The custom op <c>image_resize</c> (Java <c>org.nd4j.linalg.api.ops.impl.image.ImageResize</c>).</summary>
    Public Class ImageResize
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "image_resize"
        End Function

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ImageResize(@NonNull SameDiff sameDiff, @NonNull SDVariable in, @NonNull SDVariable size, boolean preserveAspectRatio, boolean antialias, org.nd4j.enums.ImageResizeMethod method)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal size As SDVariable, ByVal preserveAspectRatio As Boolean, ByVal antialias As Boolean, ByVal method As ImageResizeMethod)
			MyBase.New("image_resize", sameDiff, New SDVariable(){[in], size})
			addBArgument(preserveAspectRatio, antialias)
			addIArgument(method.ordinal())
		End Sub
#End If

        Public Sub New(ByVal [in] As INDArray, ByVal size As INDArray, ByVal preserveAspectRatio As Boolean, ByVal antialias As Boolean, ByVal method As ImageResizeMethod)
            MyBase.New("image_resize", New INDArray(){[in], size}, Nothing)
            Preconditions.checkArgument([in].rank()=4,"expected input message in NHWC format i.e [batchSize, height, width, channels]")
            addBArgument(preserveAspectRatio, antialias)
            addIArgument(CInt(method))
        End Sub

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes, got %s", dataTypes)
            Preconditions.checkArgument(dataTypes(0).isFPType(), "Input datatype must be floating point, got %s", dataTypes)

            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
