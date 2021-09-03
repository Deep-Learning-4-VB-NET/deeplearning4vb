﻿Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
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

Namespace org.deeplearning4j.nn.layers.convolution.subsampling

	''' <summary>
	''' Helper for the subsampling layer.
	''' 
	''' @author saudet
	''' </summary>
	Public Interface SubsamplingHelper
		Inherits LayerHelper

		Function backpropGradient(ByVal input As INDArray, ByVal epsilon As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal poolingType As PoolingType, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)

		Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal poolingType As PoolingType, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
	End Interface

End Namespace