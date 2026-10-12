Imports System.Collections.Generic
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization

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

Namespace org.deeplearning4j.nn.api


	Public Interface TrainingConfig

		''' <returns> Name of the layer </returns>
		ReadOnly Property LayerName As String

		''' <summary>
		''' Get the regularization types (l1/l2/weight decay) for the given parameter. Different parameters may have different
		''' regularization types.
		''' </summary>
		''' <param name="paramName"> Parameter name ("W", "b" etc) </param>
		''' <returns> Regularization types (if any) for the specified parameter </returns>
		Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)

		''' <summary>
		''' Is the specified parameter a layerwise pretraining only parameter?<br>
		''' For example, visible bias params in an autoencoder (or, decoder params in a variational autoencoder) aren't
		''' used during supervised backprop.<br>
		''' Layers (like DenseLayer, etc) with no pretrainable parameters will return false for all (valid) inputs.
		''' </summary>
		''' <param name="paramName"> Parameter name/key </param>
		''' <returns> True if the parameter is for layerwise pretraining only, false otherwise </returns>
		Function isPretrainParam(ByVal paramName As String) As Boolean

		''' <summary>
		''' Get the updater for the given parameter. Typically the same updater will be used for all updaters, but this
		''' is not necessarily the case
		''' </summary>
		''' <param name="paramName"> Parameter name </param>
		''' <returns> IUpdater for the parameter </returns>
		Function getUpdaterByParam(ByVal paramName As String) As IUpdater

		''' <returns> The gradient normalization configuration </returns>
		ReadOnly Property GradientNormalization As GradientNormalization

		''' <returns> The gradient normalization threshold </returns>
		ReadOnly Property GradientNormalizationThreshold As Double

		WriteOnly Property DataType As DataType

	End Interface

End Namespace