Imports System
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseAggregate = org.nd4j.linalg.api.ops.aggregates.BaseAggregate
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

Namespace org.nd4j.linalg.api.ops.aggregates.impl

	<Obsolete>
	Public Class AggregateAxpy
		Inherits BaseAggregate

		Private vectorLength As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AggregateAxpy(@NonNull INDArray x, @NonNull INDArray y, double alpha)
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal alpha As Double)
			Me.arguments.Add(x)
			Me.arguments.Add(y)

			Me.indexingArguments.Add(CInt(x.length()))

			Me.realArguments.Add(alpha)
			Me.vectorLength = CInt(x.length())
		End Sub

		''' <summary>
		''' This method returns amount of shared memory required for this specific Aggregate.
		''' PLEASE NOTE: this method is especially important for CUDA backend. On
		''' CPU backend it might be ignored, depending on Aggregate.
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property SharedMemorySize As Integer
			Get
				Return (ThreadsPerInstance * Nd4j.sizeOfDataType()) + 256
			End Get
		End Property

		''' <summary>
		''' This method returns desired number of threads per Aggregate instance
		''' PLEASE NOTE: this method is especially important for
		''' CUDA backend. On CPU backend it might be ignored,
		''' depending on Aggregate.
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property ThreadsPerInstance As Integer
			Get
				Return Math.Min(768, vectorLength)
			End Get
		End Property

		Public Overrides Function name() As String
			Return "aggregate_axpy"
		End Function

		Public Overrides Function opNum() As Integer
			Return 2
		End Function


		Public Overrides Function maxArguments() As Integer
			Return 2
		End Function

		Public Overrides Function maxShapes() As Integer
			Return 0
		End Function

		Public Overrides Function maxIntArrays() As Integer
			Return 0
		End Function

		Public Overrides Function maxIntArraySize() As Integer
			Return 0
		End Function

		Public Overrides Function maxIndexArguments() As Integer
			Return 2
		End Function

		Public Overrides Function maxRealArguments() As Integer
			Return 2
		End Function
	End Class

End Namespace