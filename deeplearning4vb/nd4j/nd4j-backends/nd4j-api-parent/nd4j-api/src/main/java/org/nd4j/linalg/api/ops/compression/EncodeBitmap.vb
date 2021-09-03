Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.api.ops.compression


	Public Class EncodeBitmap
		Inherits DynamicCustomOp

		Protected Friend threshold As Single = 1e-3f

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EncodeBitmap(@NonNull INDArray updates, float threshold)
		Public Sub New(ByVal updates As INDArray, ByVal threshold As Single)
			Me.New(updates, Nd4j.create(DataType.INT32, updates.length() \ 16 + 5), Nd4j.scalar(DataType.INT32, 0), threshold)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EncodeBitmap(@NonNull INDArray updates, @NonNull INDArray encoded, @NonNull INDArray counter, float threshold)
		Public Sub New(ByVal updates As INDArray, ByVal encoded As INDArray, ByVal counter As INDArray, ByVal threshold As Single)
			addInputArgument(updates)
			addOutputArgument(updates, encoded, counter)
			addTArgument(threshold)

			Me.threshold = threshold

			' this op ALWAYS modifies updates array
			setInPlace(True)
		End Sub

		Public Overrides Function opName() As String
			Return "encode_bitmap"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Return New List(Of DataType) From {inputArguments(0).dataType(), DataType.INT32, DataType.INT32}
		End Function
	End Class

End Namespace