Imports System
Imports Model = org.deeplearning4j.nn.api.Model
Imports TrainingHook = org.deeplearning4j.spark.api.TrainingHook
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet

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

Namespace org.deeplearning4j.spark.parameterserver

	''' <summary>
	''' Training hook for the
	''' parameter server
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class ParameterServerTrainingHook
		Implements TrainingHook

		''' <summary>
		''' A hook method for pre update.
		''' </summary>
		''' <param name="minibatch"> the inibatch
		'''                  that was used for the update </param>
		''' <param name="model">     themodel that was update </param>
		Public Overridable Sub preUpdate(ByVal minibatch As DataSet, ByVal model As Model) Implements TrainingHook.preUpdate
			'pull
		End Sub

		''' <summary>
		''' A hook method for post update
		''' </summary>
		''' <param name="minibatch"> the minibatch
		'''                  that was usd for the update </param>
		''' <param name="model">     the model that was updated </param>
		Public Overridable Sub postUpdate(ByVal minibatch As DataSet, ByVal model As Model) Implements TrainingHook.postUpdate
			'push
		End Sub

		''' <summary>
		''' A hook method for pre update.
		''' </summary>
		''' <param name="minibatch"> the inibatch
		'''                  that was used for the update </param>
		''' <param name="model">     themodel that was update </param>
		Public Overridable Sub preUpdate(ByVal minibatch As MultiDataSet, ByVal model As Model) Implements TrainingHook.preUpdate
			'pull
		End Sub

		''' <summary>
		''' A hook method for post update
		''' </summary>
		''' <param name="minibatch"> the minibatch
		'''                  that was usd for the update </param>
		''' <param name="model">     the model that was updated </param>
		Public Overridable Sub postUpdate(ByVal minibatch As MultiDataSet, ByVal model As Model) Implements TrainingHook.postUpdate
			'push
		End Sub
	End Class

End Namespace