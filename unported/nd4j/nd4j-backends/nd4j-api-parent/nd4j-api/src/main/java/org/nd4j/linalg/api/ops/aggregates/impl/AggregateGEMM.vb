Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseAggregate = org.nd4j.linalg.api.ops.aggregates.BaseAggregate

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

	Public Class AggregateGEMM
		Inherits BaseAggregate

		Public Sub New()
			' no-op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AggregateGEMM(int Order, int TransA, int TransB, int M, int N, int K, double alpha, @NonNull INDArray A, int lda, @NonNull INDArray B, int ldb, double beta, @NonNull INDArray C, int ldc)
		Public Sub New(ByVal Order As Integer, ByVal TransA As Integer, ByVal TransB As Integer, ByVal M As Integer, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)
			Me.arguments.Add(A)
			Me.arguments.Add(B)
			Me.arguments.Add(C)

			Me.indexingArguments.Add(M)
			Me.indexingArguments.Add(N)
			Me.indexingArguments.Add(K)
			Me.indexingArguments.Add(lda)
			Me.indexingArguments.Add(ldb)
			Me.indexingArguments.Add(ldc)
			Me.indexingArguments.Add(TransA)
			Me.indexingArguments.Add(TransB)
			Me.indexingArguments.Add(Order)

			Me.realArguments.Add(alpha)
			Me.realArguments.Add(beta)
		End Sub

		Public Overrides Function name() As String
			Return "aggregate_gemm"
		End Function

		Public Overrides Function opNum() As Integer
			Return 5
		End Function

		Public Overrides Function maxArguments() As Integer
			Return 3
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
			Return 9
		End Function

		Public Overrides Function maxRealArguments() As Integer
			Return 2
		End Function

		Public Overrides ReadOnly Property SharedMemorySize As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides ReadOnly Property ThreadsPerInstance As Integer
			Get
				Return 0
			End Get
		End Property
	End Class

End Namespace