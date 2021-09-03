Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports org.junit.jupiter.api.Assertions

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.LOSS_FUNCTIONS) @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestLossFunctionsSizeChecks extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestLossFunctionsSizeChecks
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testL2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testL2(ByVal backend As Nd4jBackend)
			Dim lossFunctionList() As LossFunction = {LossFunction.MSE, LossFunction.L1, LossFunction.XENT, LossFunction.MCXENT, LossFunction.SQUARED_LOSS, LossFunction.RECONSTRUCTION_CROSSENTROPY, LossFunction.NEGATIVELOGLIKELIHOOD, LossFunction.COSINE_PROXIMITY, LossFunction.HINGE, LossFunction.SQUARED_HINGE, LossFunction.KL_DIVERGENCE, LossFunction.MEAN_ABSOLUTE_ERROR, LossFunction.L2, LossFunction.MEAN_ABSOLUTE_PERCENTAGE_ERROR, LossFunction.MEAN_SQUARED_LOGARITHMIC_ERROR, LossFunction.POISSON}

			testLossFunctions(lossFunctionList)
		End Sub

		Public Overridable Sub testLossFunctions(ByVal lossFunctions() As LossFunction)
			For Each loss As LossFunction In lossFunctions
				testLossFunctionScoreSizeMismatchCase(loss.getILossFunction())
			Next loss
		End Sub

		''' <summary>
		''' This method checks that the given loss function will give an assertion
		''' if the labels and output vectors are of different sizes. </summary>
		''' <param name="loss"> Loss function to verify. </param>
		Public Overridable Sub testLossFunctionScoreSizeMismatchCase(ByVal loss As ILossFunction)

			Try
				Dim labels As INDArray = Nd4j.create(100, 32)
				Dim preOutput As INDArray = Nd4j.create(100, 44)
				Dim score As Double = loss.computeScore(labels, preOutput, Activation.IDENTITY.getActivationFunction(), Nothing, True)
				assertFalse(True, "Loss function " & loss.ToString() & "did not check for size mismatch.  This should fail to compute an activation function because the sizes of the vectors are not equal")
			Catch ex As System.ArgumentException
				Dim exceptionMessage As String = ex.Message
				assertTrue(exceptionMessage.Contains("shapes"), "Loss function exception " & loss.ToString() & " did not indicate size mismatch when vectors of incorrect size were used.")
			End Try

			Try
				Dim labels As INDArray = Nd4j.create(100, 32)
				Dim preOutput As INDArray = Nd4j.create(100, 44)
				Dim gradient As INDArray = loss.computeGradient(labels, preOutput, Activation.IDENTITY.getActivationFunction(), Nothing)
				assertFalse(True, "Loss function " & loss.ToString() & "did not check for size mismatch.  This should fail to compute an activation function because the sizes of the vectors are not equal")
			Catch ex As System.ArgumentException
				Dim exceptionMessage As String = ex.Message
				assertTrue(exceptionMessage.Contains("shapes"), "Loss function exception " & loss.ToString() & " did not indicate size mismatch when vectors of incorrect size were used.")
			End Try

		End Sub
	End Class

End Namespace