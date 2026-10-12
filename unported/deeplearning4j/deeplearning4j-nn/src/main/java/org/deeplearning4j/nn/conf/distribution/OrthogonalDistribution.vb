Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports JsonCreator = org.nd4j.shade.jackson.annotation.JsonCreator
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.nn.conf.distribution

	''' <summary>
	''' Orthogonal distribution, with gain parameter.<br>
	''' See <a href="https://arxiv.org/abs/1312.6120">https://arxiv.org/abs/1312.6120</a> for details
	''' 
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Data public class OrthogonalDistribution extends Distribution
	<Serializable>
	Public Class OrthogonalDistribution
		Inherits Distribution

		Private gain As Double

		''' <summary>
		''' Create a log-normal distribution
		''' with the given mean and std
		''' </summary>
		''' <param name="gain"> the gain </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public OrthogonalDistribution(@JsonProperty("gain") double gain)
		Public Sub New(ByVal gain As Double)
			Me.gain = gain
		End Sub

		Public Overrides Function ToString() As String
			Return "OrthogonalDistribution(gain=" & gain & ")"
		End Function
	End Class

End Namespace