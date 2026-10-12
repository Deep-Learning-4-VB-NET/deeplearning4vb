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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Data public class TruncatedNormalDistribution extends Distribution
	<Serializable>
	Public Class TruncatedNormalDistribution
		Inherits Distribution

		Private mean, std As Double

		''' <summary>
		''' Create a truncated normal distribution
		''' with the given mean and std
		''' </summary>
		''' <param name="mean"> the mean </param>
		''' <param name="std">  the standard deviation </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public TruncatedNormalDistribution(@JsonProperty("mean") double mean, @JsonProperty("std") double std)
		Public Sub New(ByVal mean As Double, ByVal std As Double)
			Me.mean = mean
			Me.std = std
		End Sub

		Public Overrides Function ToString() As String
			Return "TruncatedNormalDistribution(" & "mean=" & mean & ", std=" & std & ")"c
		End Function
	End Class

End Namespace