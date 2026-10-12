Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports WeightInit = org.nd4j.weightinit.WeightInit
Imports WeightInitScheme = org.nd4j.weightinit.WeightInitScheme

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

Namespace org.nd4j.weightinit.impl

	''' 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class NDArraySupplierInitScheme implements org.nd4j.weightinit.WeightInitScheme
	Public Class NDArraySupplierInitScheme
		Implements WeightInitScheme

		Private supplier As NDArraySupplier

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public NDArraySupplierInitScheme(final org.nd4j.linalg.api.ndarray.INDArray arr)
		Public Sub New(ByVal arr As INDArray)
			Me.New(New NDArraySupplierAnonymousInnerClass(Me))
		End Sub

		Private Class NDArraySupplierAnonymousInnerClass
			Implements NDArraySupplierInitScheme.NDArraySupplier

			Private ReadOnly outerInstance As NDArraySupplierInitScheme

			Public Sub New(ByVal outerInstance As NDArraySupplierInitScheme)
				Me.outerInstance = outerInstance
			End Sub

			Public ReadOnly Property Arr As INDArray Implements NDArraySupplierInitScheme.NDArraySupplier.getArr
				Get
					Return arr
				End Get
			End Property
		End Class

		''' <summary>
		''' A simple <seealso cref="INDArray facade"/>
		''' </summary>
		Public Interface NDArraySupplier
			''' <summary>
			''' An array proxy method.
			''' @return
			''' </summary>
			ReadOnly Property Arr As INDArray
		End Interface

		Public Overridable Function create(ByVal shape() As Long, ByVal paramsView As INDArray) As INDArray Implements WeightInitScheme.create
			Return supplier.Arr
		End Function

		Public Overridable Function create(ByVal dataType As DataType, ByVal shape() As Long) As INDArray Implements WeightInitScheme.create
			Return supplier.Arr
		End Function

		Public Overridable Function order() As Char Implements WeightInitScheme.order
			Return "f"c
		End Function

		Public Overridable Function type() As WeightInit
			Return WeightInit.SUPPLIED
		End Function
	End Class

End Namespace