Imports Data = lombok.Data

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

Namespace org.deeplearning4j.nn.modelimport.keras.config


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class KerasModelConfiguration
	Public Class KerasModelConfiguration

		' Model meta information fields 
		Private ReadOnly fieldClassName As String = "class_name"
		Private ReadOnly fieldClassNameSequential As String = "Sequential"
		Private ReadOnly fieldClassNameModel As String = "Model"
		Private ReadOnly fieldNameClassFunctional As String = "Functional"
		Private ReadOnly fieldKerasVersion As String = "keras_version"
		Private ReadOnly fieldBackend As String = "backend"


		' Model configuration field. 
		Private ReadOnly modelFieldConfig As String = "config"
		Private ReadOnly modelFieldLayers As String = "layers"
		Private ReadOnly modelFieldInputLayers As String = "input_layers"
		Private ReadOnly modelFieldOutputLayers As String = "output_layers"

		' Training configuration field. 
		Private ReadOnly trainingLoss As String = "loss"
		Private ReadOnly trainingWeightsRoot As String = "model_weights"
		Private ReadOnly trainingModelConfigAttribute As String = "model_config"
		Private ReadOnly trainingTrainingConfigAttribute As String = "training_config"
		Private ReadOnly optimizerConfig As String = "optimizer_config"

	End Class

End Namespace