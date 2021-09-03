Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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

Namespace org.deeplearning4j.nn.layers.normalization

	Public Interface BatchNormalizationHelper
		Inherits LayerHelper

		Function checkSupported(ByVal eps As Double, ByVal fixedGammaBeta As Boolean) As Boolean

		Function backpropGradient(ByVal input As INDArray, ByVal epsilon As INDArray, ByVal shape() As Long, ByVal gamma As INDArray, ByVal beta As INDArray, ByVal dGammaView As INDArray, ByVal dBetaView As INDArray, ByVal eps As Double, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)

		Function preOutput(ByVal x As INDArray, ByVal training As Boolean, ByVal shape() As Long, ByVal gamma As INDArray, ByVal beta As INDArray, ByVal mean As INDArray, ByVal var As INDArray, ByVal decay As Double, ByVal eps As Double, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

		Function getMeanCache(ByVal dataType As DataType) As INDArray

		Function getVarCache(ByVal dataType As DataType) As INDArray
	End Interface

End Namespace