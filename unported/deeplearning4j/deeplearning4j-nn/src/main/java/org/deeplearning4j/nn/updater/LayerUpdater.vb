Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Trainable = org.deeplearning4j.nn.api.Trainable
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.nn.updater


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class LayerUpdater extends BaseMultiLayerUpdater<org.deeplearning4j.nn.api.Layer>
	<Serializable>
	Public Class LayerUpdater
		Inherits BaseMultiLayerUpdater(Of Layer)

		Public Sub New(ByVal layer As Layer)
			Me.New(layer, Nothing)
		End Sub

		Public Sub New(ByVal layer As Layer, ByVal updaterState As INDArray)
			MyBase.New(layer, updaterState)
			If TypeOf layer Is MultiLayerNetwork Then
				Throw New System.NotSupportedException("Cannot use LayerUpdater for a MultiLayerNetwork")
			End If

			layersByName = New Dictionary(Of String, Trainable)()
			layersByName(layer.conf().getLayer().getLayerName()) = layer
		End Sub

		Protected Friend Overrides ReadOnly Property OrderedLayers As Trainable()
			Get
				Return New Trainable() {DirectCast(network, Trainable)}
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property FlattenedGradientsView As INDArray
			Get
				Return network.GradientsViewArray
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property Params As INDArray
			Get
				Return network.params()
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property MiniBatch As Boolean
			Get
				Return network.conf().isMiniBatch()
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property SingleLayerUpdater As Boolean
			Get
				Return True
			End Get
		End Property
	End Class

End Namespace