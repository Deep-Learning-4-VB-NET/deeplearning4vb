Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ImageResizeMethod = org.nd4j.enums.ImageResizeMethod
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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
'ORIGINAL LINE: @NoArgsConstructor public class ImageResize extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class ImageResize
		Inherits DynamicCustomOp

		Public Overrides Function opName() As String
			Return "image_resize"
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ImageResize(@NonNull SameDiff sameDiff, @NonNull SDVariable in, @NonNull SDVariable size, boolean preserveAspectRatio, boolean antialias, org.nd4j.enums.ImageResizeMethod method)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal size As SDVariable, ByVal preserveAspectRatio As Boolean, ByVal antialias As Boolean, ByVal method As ImageResizeMethod)
			MyBase.New("image_resize", sameDiff, New SDVariable(){[in], size})
			addBArgument(preserveAspectRatio, antialias)
			addIArgument(method.ordinal())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ImageResize(@NonNull INDArray in, @NonNull INDArray size, boolean preserveAspectRatio, boolean antialias, org.nd4j.enums.ImageResizeMethod method)
		Public Sub New(ByVal [in] As INDArray, ByVal size As INDArray, ByVal preserveAspectRatio As Boolean, ByVal antialias As Boolean, ByVal method As ImageResizeMethod)
			MyBase.New("image_resize", New INDArray(){[in], size}, Nothing)
			Preconditions.checkArgument([in].rank()=4,"expected input message in NHWC format i.e [batchSize, height, width, channels]")
			addBArgument(preserveAspectRatio, antialias)
			addIArgument(method.ordinal())
		End Sub



		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes, got %s", dataTypes)
			Preconditions.checkArgument(dataTypes(0).isFPType(), "Input datatype must be floating point, got %s", dataTypes)

			Return Collections.singletonList(dataTypes(0))
		End Function


	End Class
End Namespace