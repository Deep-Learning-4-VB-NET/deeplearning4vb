Imports Getter = lombok.Getter
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
Namespace org.deeplearning4j.rl4j.agent.learning.update

	Public Class Features

		Private ReadOnly features() As INDArray

		''' <summary>
		''' The size of the batch
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final long batchSize;
		Private ReadOnly batchSize As Long

		Public Sub New(ByVal features() As INDArray)
			Me.features = features
			batchSize = features(0).shape()(0)
		End Sub

		''' <param name="channelIdx"> The channel to get </param>
		''' <returns> A <seealso cref="INDArray"/> associated to the channel index </returns>
		Public Overridable Function get(ByVal channelIdx As Integer) As INDArray
			Return features(channelIdx)
		End Function
	End Class
End Namespace