Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
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

Namespace org.deeplearning4j.nn.conf.misc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class DummyConfig implements org.deeplearning4j.nn.api.TrainingConfig
	Public Class DummyConfig
		Implements TrainingConfig

		Private ReadOnly name As String

		Public Overridable ReadOnly Property LayerName As String Implements TrainingConfig.getLayerName
			Get
				Return name
			End Get
		End Property

		Public Overridable Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization) Implements TrainingConfig.getRegularizationByParam
			Return Nothing
		End Function

		Public Overridable Function isPretrainParam(ByVal paramName As String) As Boolean Implements TrainingConfig.isPretrainParam
			Return False
		End Function

		Public Overridable Function getUpdaterByParam(ByVal paramName As String) As IUpdater Implements TrainingConfig.getUpdaterByParam
			Return New NoOp()
		End Function

		Public Overridable ReadOnly Property GradientNormalization As GradientNormalization Implements TrainingConfig.getGradientNormalization
			Get
				Return GradientNormalization.None
			End Get
		End Property

		Public Overridable ReadOnly Property GradientNormalizationThreshold As Double Implements TrainingConfig.getGradientNormalizationThreshold
			Get
				Return 1.0
			End Get
		End Property

		Public Overridable WriteOnly Property DataType Implements TrainingConfig.setDataType As DataType
			Set(ByVal dataType As DataType)
    
			End Set
		End Property
	End Class

End Namespace