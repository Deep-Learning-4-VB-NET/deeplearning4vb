Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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

Namespace org.deeplearning4j.nn.modelimport.keras.utils



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasLossUtils
	Public Class KerasLossUtils
		Friend Shared ReadOnly customLoss As IDictionary(Of String, ILossFunction) = New Dictionary(Of String, ILossFunction)()

		''' <summary>
		''' Register a custom loss function
		''' </summary>
		''' <param name="lossName">   name of the lambda layer in the serialized Keras model </param>
		''' <param name="lossFunction"> SameDiffLambdaLayer instance to map to Keras Lambda layer </param>
		Public Shared Sub registerCustomLoss(ByVal lossName As String, ByVal lossFunction As ILossFunction)
			customLoss(lossName) = lossFunction
		End Sub

		''' <summary>
		''' Clear all lambda layers
		''' 
		''' </summary>
		Public Shared Sub clearCustomLoss()
			customLoss.Clear()
		End Sub

		''' <summary>
		''' Map Keras to DL4J loss functions.
		''' </summary>
		''' <param name="kerasLoss"> String containing Keras loss function name </param>
		''' <returns> String containing DL4J loss function </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.linalg.lossfunctions.ILossFunction mapLossFunction(String kerasLoss, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function mapLossFunction(ByVal kerasLoss As String, ByVal conf As KerasLayerConfiguration) As ILossFunction
			Dim dl4jLoss As LossFunctions.LossFunction
			kerasLoss = kerasLoss.ToLower()
			If kerasLoss.Equals(conf.getKERAS_LOSS_MEAN_SQUARED_ERROR()) OrElse kerasLoss.Equals(conf.getKERAS_LOSS_MSE()) Then
				dl4jLoss = LossFunctions.LossFunction.SQUARED_LOSS
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_MEAN_ABSOLUTE_ERROR()) OrElse kerasLoss.Equals(conf.getKERAS_LOSS_MAE()) Then
				dl4jLoss = LossFunctions.LossFunction.MEAN_ABSOLUTE_ERROR
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_MEAN_ABSOLUTE_PERCENTAGE_ERROR()) OrElse kerasLoss.Equals(conf.getKERAS_LOSS_MAPE()) Then
				dl4jLoss = LossFunctions.LossFunction.MEAN_ABSOLUTE_PERCENTAGE_ERROR
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_MEAN_SQUARED_LOGARITHMIC_ERROR()) OrElse kerasLoss.Equals(conf.getKERAS_LOSS_MSLE()) Then
				dl4jLoss = LossFunctions.LossFunction.MEAN_SQUARED_LOGARITHMIC_ERROR
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_SQUARED_HINGE()) Then
				dl4jLoss = LossFunctions.LossFunction.SQUARED_HINGE
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_HINGE()) Then
				dl4jLoss = LossFunctions.LossFunction.HINGE
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_SPARSE_CATEGORICAL_CROSSENTROPY()) Then
				dl4jLoss = LossFunctions.LossFunction.SPARSE_MCXENT
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_BINARY_CROSSENTROPY()) Then
				dl4jLoss = LossFunctions.LossFunction.XENT
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_CATEGORICAL_CROSSENTROPY()) Then
				dl4jLoss = LossFunctions.LossFunction.MCXENT
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_KULLBACK_LEIBLER_DIVERGENCE()) OrElse kerasLoss.Equals(conf.getKERAS_LOSS_KLD()) Then
				dl4jLoss = LossFunctions.LossFunction.KL_DIVERGENCE
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_POISSON()) Then
				dl4jLoss = LossFunctions.LossFunction.POISSON
			ElseIf kerasLoss.Equals(conf.getKERAS_LOSS_COSINE_PROXIMITY()) Then
				dl4jLoss = LossFunctions.LossFunction.COSINE_PROXIMITY
			Else
				Dim lossClass As ILossFunction = customLoss(kerasLoss)
				If lossClass IsNot Nothing Then
					Return lossClass
				Else
					Throw New UnsupportedKerasConfigurationException("Unknown Keras loss function " & kerasLoss)
				End If
			End If
			Return dl4jLoss.getILossFunction()
		End Function
	End Class

End Namespace