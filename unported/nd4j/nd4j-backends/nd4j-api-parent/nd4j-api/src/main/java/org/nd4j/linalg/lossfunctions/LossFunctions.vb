Imports System.Collections.Generic
Imports org.nd4j.linalg.lossfunctions.impl

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

Namespace org.nd4j.linalg.lossfunctions


	Public Class LossFunctions

		''' <summary>
		''' MSE: Mean Squared Error: Linear Regression - <seealso cref="LossMSE"/><br>
		''' l1: L1 loss (absolute value) - <seealso cref="LossL1"/><br>
		''' XENT: Cross Entropy: Binary Classification - <seealso cref="LossBinaryXENT"/><br>
		''' MCXENT: Multiclass Cross Entropy - <seealso cref="LossMCXENT"/><br>
		''' SPARSE_MCXENT: Sparse multi-class cross entropy - <seealso cref="LossSparseMCXENT"/><br>
		''' SQUARED_LOSS: Alias for mean squared error - <seealso cref="LossMSE"/><br>
		''' NEGATIVELOGLIKELIHOOD: Negative Log Likelihood - <seealso cref="LossNegativeLogLikelihood"/><br>
		''' COSINE_PROXIMITY: Cosine proximity loss - <seealso cref="LossCosineProximity"/><br>
		''' HINGE: Hinge loss - <seealso cref="LossHinge"/><br>
		''' SQUARED_HINGE: Squared hinge loss - <seealso cref="LossSquaredHinge"/><br>
		''' KL_DIVERGENCE: Kullback-Leibler divergence loss - <seealso cref="LossKLD"/><br>
		''' MEAN_ABSOLUTE_ERROR: mean absolute error loss - <seealso cref="LossMAE"/><br>
		''' L2: L2 loss (sum of squared errors) - <seealso cref="LossL2"/><br>
		''' MEAN_ABSOLUTE_PERCENTAGE_ERROR: MAPE loss - <seealso cref="LossMAPE"/><br>
		''' MEAN_SQUARED_LOGARITHMIC_ERROR: MSLE loss - <seealso cref="LossMSLE"/><br>
		''' POISSON: Poisson loss - <seealso cref="LossPoisson"/><br>
		''' WASSERSTEIN: Wasserstein loss - <seealso cref="LossWasserstein"/>
		''' </summary>
		Public NotInheritable Class LossFunction
			Public Shared ReadOnly MSE As New LossFunction("MSE", InnerEnum.MSE)
			Public Shared ReadOnly L1 As New LossFunction("L1", InnerEnum.L1)
			Public Shared ReadOnly XENT As New LossFunction("XENT", InnerEnum.XENT)
			Public Shared ReadOnly MCXENT As New LossFunction("MCXENT", InnerEnum.MCXENT)
			Public Shared ReadOnly SPARSE_MCXENT As New LossFunction("SPARSE_MCXENT", InnerEnum.SPARSE_MCXENT)
			Public Shared ReadOnly SQUARED_LOSS As New LossFunction("SQUARED_LOSS", InnerEnum.SQUARED_LOSS)
			Public Shared ReadOnly RECONSTRUCTION_CROSSENTROPY As New LossFunction("RECONSTRUCTION_CROSSENTROPY", InnerEnum.RECONSTRUCTION_CROSSENTROPY)
			Public Shared ReadOnly NEGATIVELOGLIKELIHOOD As New LossFunction("NEGATIVELOGLIKELIHOOD", InnerEnum.NEGATIVELOGLIKELIHOOD)
			Public Shared ReadOnly COSINE_PROXIMITY As New LossFunction("COSINE_PROXIMITY", InnerEnum.COSINE_PROXIMITY)
			Public Shared ReadOnly HINGE As New LossFunction("HINGE", InnerEnum.HINGE)
			Public Shared ReadOnly SQUARED_HINGE As New LossFunction("SQUARED_HINGE", InnerEnum.SQUARED_HINGE)
			Public Shared ReadOnly KL_DIVERGENCE As New LossFunction("KL_DIVERGENCE", InnerEnum.KL_DIVERGENCE)
			Public Shared ReadOnly MEAN_ABSOLUTE_ERROR As New LossFunction("MEAN_ABSOLUTE_ERROR", InnerEnum.MEAN_ABSOLUTE_ERROR)
			Public Shared ReadOnly L2 As New LossFunction("L2", InnerEnum.L2)
			Public Shared ReadOnly MEAN_ABSOLUTE_PERCENTAGE_ERROR As New LossFunction("MEAN_ABSOLUTE_PERCENTAGE_ERROR", InnerEnum.MEAN_ABSOLUTE_PERCENTAGE_ERROR)
			Public Shared ReadOnly MEAN_SQUARED_LOGARITHMIC_ERROR As New LossFunction("MEAN_SQUARED_LOGARITHMIC_ERROR", InnerEnum.MEAN_SQUARED_LOGARITHMIC_ERROR)
			Public Shared ReadOnly POISSON As New LossFunction("POISSON", InnerEnum.POISSON)
			Public Shared ReadOnly WASSERSTEIN As New LossFunction("WASSERSTEIN", InnerEnum.WASSERSTEIN)

			Private Shared ReadOnly valueList As New List(Of LossFunction)()

			Shared Sub New()
				valueList.Add(MSE)
				valueList.Add(L1)
				valueList.Add(XENT)
				valueList.Add(MCXENT)
				valueList.Add(SPARSE_MCXENT)
				valueList.Add(SQUARED_LOSS)
				valueList.Add(RECONSTRUCTION_CROSSENTROPY)
				valueList.Add(NEGATIVELOGLIKELIHOOD)
				valueList.Add(COSINE_PROXIMITY)
				valueList.Add(HINGE)
				valueList.Add(SQUARED_HINGE)
				valueList.Add(KL_DIVERGENCE)
				valueList.Add(MEAN_ABSOLUTE_ERROR)
				valueList.Add(L2)
				valueList.Add(MEAN_ABSOLUTE_PERCENTAGE_ERROR)
				valueList.Add(MEAN_SQUARED_LOGARITHMIC_ERROR)
				valueList.Add(POISSON)
				valueList.Add(WASSERSTEIN)
			End Sub

			Public Enum InnerEnum
				MSE
				L1
				XENT
				MCXENT
				SPARSE_MCXENT
				SQUARED_LOSS
				RECONSTRUCTION_CROSSENTROPY
				NEGATIVELOGLIKELIHOOD
				COSINE_PROXIMITY
				HINGE
				SQUARED_HINGE
				KL_DIVERGENCE
				MEAN_ABSOLUTE_ERROR
				L2
				MEAN_ABSOLUTE_PERCENTAGE_ERROR
				MEAN_SQUARED_LOGARITHMIC_ERROR
				POISSON
				WASSERSTEIN
			End Enum

			Public ReadOnly innerEnumValue As InnerEnum
			Private ReadOnly nameValue As String
			Private ReadOnly ordinalValue As Integer
			Private Shared nextOrdinal As Integer = 0

			Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
				nameValue = name
				ordinalValue = nextOrdinal
				nextOrdinal += 1
				innerEnumValue = thisInnerEnumValue
			End Sub

			Public ReadOnly Property ILossFunction As ILossFunction
				Get
					Select Case Me
						Case MSE, SQUARED_LOSS
							Return New LossMSE()
						Case L1
							Return New LossL1()
						Case XENT
							Return New LossBinaryXENT()
						Case MCXENT
							Return New LossMCXENT()
						Case SPARSE_MCXENT
							Return New LossSparseMCXENT()
						Case KL_DIVERGENCE, RECONSTRUCTION_CROSSENTROPY
							Return New LossKLD()
						Case NEGATIVELOGLIKELIHOOD
							Return New LossNegativeLogLikelihood()
						Case COSINE_PROXIMITY
							Return New LossCosineProximity()
						Case HINGE
							Return New LossHinge()
						Case SQUARED_HINGE
							Return New LossSquaredHinge()
						Case MEAN_ABSOLUTE_ERROR
							Return New LossMAE()
						Case L2
							Return New LossL2()
						Case MEAN_ABSOLUTE_PERCENTAGE_ERROR
							Return New LossMAPE()
						Case MEAN_SQUARED_LOGARITHMIC_ERROR
							Return New LossMSLE()
						Case POISSON
							Return New LossPoisson()
						Case WASSERSTEIN
							Return New LossWasserstein()
						Case Else
							'Custom, RMSE_XENT
							Throw New System.NotSupportedException("Unknown or not supported loss function: " & Me)
					End Select
				End Get
			End Property

			Public Shared Function values() As LossFunction()
				Return valueList.ToArray()
			End Function

			Public Function ordinal() As Integer
				Return ordinalValue
			End Function

			Public Overrides Function ToString() As String
				Return nameValue
			End Function

			Public Shared Operator =(ByVal one As LossFunction, ByVal two As LossFunction) As Boolean
				Return one.innerEnumValue = two.innerEnumValue
			End Operator

			Public Shared Operator <>(ByVal one As LossFunction, ByVal two As LossFunction) As Boolean
				Return one.innerEnumValue <> two.innerEnumValue
			End Operator

			Public Shared Function valueOf(ByVal name As String) As LossFunction
				For Each enumInstance As LossFunction In LossFunction.valueList
					If enumInstance.nameValue = name Then
						Return enumInstance
					End If
				Next
				Throw New System.ArgumentException(name)
			End Function
		End Class


	End Class

End Namespace