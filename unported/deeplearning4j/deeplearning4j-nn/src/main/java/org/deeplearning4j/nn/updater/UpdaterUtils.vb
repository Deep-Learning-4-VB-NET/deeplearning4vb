Imports Trainable = org.deeplearning4j.nn.api.Trainable
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater

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

	Public Class UpdaterUtils


		Public Shared Function updaterConfigurationsEquals(ByVal layer1 As Trainable, ByVal param1 As String, ByVal layer2 As Trainable, ByVal param2 As String) As Boolean
			Dim l1 As TrainingConfig = layer1.Config
			Dim l2 As TrainingConfig = layer2.Config
			Dim u1 As IUpdater = l1.getUpdaterByParam(param1)
			Dim u2 As IUpdater = l2.getUpdaterByParam(param2)

			'For updaters to be equal (and hence combinable), we require that:
			'(a) The updater-specific configurations are equal (inc. LR, LR/momentum schedules etc)
			'(b) If one or more of the params are pretrainable params, they are in the same layer
			'    This last point is necessary as we don't want to modify the pretrain gradient/updater state during
			'    backprop, or modify the pretrain gradient/updater state of one layer while training another
			If Not u1.Equals(u2) Then
				'Different updaters or different config
				Return False
			End If

			Dim isPretrainParam1 As Boolean = l1.isPretrainParam(param1)
			Dim isPretrainParam2 As Boolean = l2.isPretrainParam(param2)
			If isPretrainParam1 OrElse isPretrainParam2 Then
				'One or both of params are pretrainable.
				'Either layers differ -> don't want to combine a pretrain updaters across layers
				'Or one is pretrain and the other isn't -> don't want to combine pretrain updaters within a layer
				Return layer1 Is layer2 AndAlso isPretrainParam1 AndAlso isPretrainParam2
			End If

			Return True
		End Function
	End Class

End Namespace