Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException
Imports LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats
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
	''' A uniform distribution, with two parameters: lower and upper - i.e., U(lower,upper)
	''' 
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Data public class UniformDistribution extends Distribution
	<Serializable>
	Public Class UniformDistribution
		Inherits Distribution

		Private upper, lower As Double

		''' <summary>
		''' Create a uniform real distribution using the given lower and upper
		''' bounds.
		''' </summary>
		''' <param name="lower"> Lower bound of this distribution (inclusive). </param>
		''' <param name="upper"> Upper bound of this distribution (exclusive). </param>
		''' <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public UniformDistribution(@JsonProperty("lower") double lower, @JsonProperty("upper") double upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Sub New(ByVal lower As Double, ByVal upper As Double)
			If lower >= upper Then
				Throw New NumberIsTooLargeException(LocalizedFormats.LOWER_BOUND_NOT_BELOW_UPPER_BOUND, lower, upper, False)
			End If
			Me.lower = lower
			Me.upper = upper
		End Sub

		Public Overrides Function ToString() As String
			Return "UniformDistribution(lower=" & lower & ", upper=" & upper & ")"
		End Function
	End Class

End Namespace