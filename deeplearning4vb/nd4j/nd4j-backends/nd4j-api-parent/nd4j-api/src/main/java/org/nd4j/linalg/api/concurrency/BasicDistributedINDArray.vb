Imports NonNull = lombok.NonNull

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

Namespace org.nd4j.linalg.api.concurrency

	Public MustInherit Class BasicDistributedINDArray
		Implements DistributedINDArray

		Public MustOverride Sub allocate(ByVal entry As Integer, ByVal dataType As org.nd4j.linalg.api.buffer.DataType, ByVal shape() As Long)
		Public MustOverride Sub allocate(ByVal entry As Integer, ByVal shapeDescriptor As org.nd4j.linalg.api.shape.LongShapeDescriptor)
		Public MustOverride Function numActiveEntries() As Integer Implements DistributedINDArray.numActiveEntries
		Public MustOverride Function numEntries() As Integer Implements DistributedINDArray.numEntries
		Public MustOverride Sub propagate(ByVal array As org.nd4j.linalg.api.ndarray.INDArray) Implements DistributedINDArray.propagate
		Public MustOverride Function entry() As org.nd4j.linalg.api.ndarray.INDArray Implements DistributedINDArray.entry
		Public MustOverride Function entry(ByVal entry As Integer) As org.nd4j.linalg.api.ndarray.INDArray
		Private ReadOnly arrayType As ArrayType

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BasicDistributedINDArray(@NonNull ArrayType arrayType)
		Public Sub New(ByVal arrayType As ArrayType)
			Me.arrayType = arrayType
		End Sub

		Public Overridable ReadOnly Property INDArrayType As ArrayType Implements DistributedINDArray.getINDArrayType
			Get
				Return arrayType
			End Get
		End Property
	End Class

End Namespace