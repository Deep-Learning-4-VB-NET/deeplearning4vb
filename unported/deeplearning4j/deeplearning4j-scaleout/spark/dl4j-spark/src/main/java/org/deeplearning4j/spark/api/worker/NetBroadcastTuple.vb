Imports System
Imports Data = lombok.Data
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
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

Namespace org.deeplearning4j.spark.api.worker


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class NetBroadcastTuple implements java.io.Serializable
	<Serializable>
	Public Class NetBroadcastTuple

		Private ReadOnly configuration As MultiLayerConfiguration
		Private ReadOnly graphConfiguration As ComputationGraphConfiguration
		Private ReadOnly parameters As INDArray
		Private ReadOnly updaterState As INDArray
		Private ReadOnly counter As AtomicInteger

		Public Sub New(ByVal configuration As MultiLayerConfiguration, ByVal parameters As INDArray, ByVal updaterState As INDArray)
			Me.New(configuration, Nothing, parameters, updaterState)
		End Sub

		Public Sub New(ByVal graphConfiguration As ComputationGraphConfiguration, ByVal parameters As INDArray, ByVal updaterState As INDArray)
			Me.New(Nothing, graphConfiguration, parameters, updaterState)

		End Sub

		Public Sub New(ByVal configuration As MultiLayerConfiguration, ByVal graphConfiguration As ComputationGraphConfiguration, ByVal parameters As INDArray, ByVal updaterState As INDArray)
			Me.New(configuration, graphConfiguration, parameters, updaterState, New AtomicInteger(0))
		End Sub

		Public Sub New(ByVal configuration As MultiLayerConfiguration, ByVal graphConfiguration As ComputationGraphConfiguration, ByVal parameters As INDArray, ByVal updaterState As INDArray, ByVal counter As AtomicInteger)
			Me.configuration = configuration
			Me.graphConfiguration = graphConfiguration
			Me.parameters = parameters
			Me.updaterState = updaterState
			Me.counter = counter
		End Sub
	End Class

End Namespace