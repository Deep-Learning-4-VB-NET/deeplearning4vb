Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports Model = org.deeplearning4j.nn.api.Model

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

Namespace org.deeplearning4j.earlystopping.listener

	Public Interface EarlyStoppingListener(Of T As org.deeplearning4j.nn.api.Model)

		''' <summary>
		'''Method to be called when early stopping training is first started
		''' </summary>
		Sub onStart(ByVal esConfig As EarlyStoppingConfiguration(Of T), ByVal net As T)

		''' <summary>
		'''Method that is called at the end of each epoch completed during early stopping training </summary>
		''' <param name="epochNum"> The number of the epoch just completed (starting at 0) </param>
		''' <param name="score"> The score calculated </param>
		''' <param name="esConfig"> Configuration </param>
		''' <param name="net"> Network (current) </param>
		Sub onEpoch(ByVal epochNum As Integer, ByVal score As Double, ByVal esConfig As EarlyStoppingConfiguration(Of T), ByVal net As T)

		''' <summary>
		'''Method that is called at the end of early stopping training </summary>
		''' <param name="esResult"> The early stopping result. Provides details of why early stopping training was terminated, etc </param>
		Sub onCompletion(ByVal esResult As EarlyStoppingResult(Of T))

	End Interface

End Namespace