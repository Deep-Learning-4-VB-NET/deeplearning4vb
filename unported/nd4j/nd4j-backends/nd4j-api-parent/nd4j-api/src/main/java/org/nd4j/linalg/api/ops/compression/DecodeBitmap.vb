Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
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

Namespace org.nd4j.linalg.api.ops.compression


	Public Class DecodeBitmap
		Inherits DynamicCustomOp

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DecodeBitmap(@NonNull INDArray encoded, @NonNull INDArray updates)
		Public Sub New(ByVal encoded As INDArray, ByVal updates As INDArray)
			addInputArgument(updates, encoded)
			addOutputArgument(updates)

			' this op ALWAYS modifies updates array
			setInPlace(True)
		End Sub

		Public Overrides Function opName() As String
			Return "decode_bitmap"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Return New List(Of DataType) From {inputArguments(0).dataType(), DataType.INT32}
		End Function
	End Class

End Namespace