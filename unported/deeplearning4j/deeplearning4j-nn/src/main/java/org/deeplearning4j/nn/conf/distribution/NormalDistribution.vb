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
	''' A normal (Gaussian) distribution, with two parameters: mean and standard deviation
	''' 
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Data public class NormalDistribution extends Distribution
	<Serializable>
	Public Class NormalDistribution
		Inherits Distribution

'JAVA TO VB CONVERTER NOTE: The field mean was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field std was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private mean_Conflict, std_Conflict As Double

		''' <summary>
		''' Create a normal distribution
		''' with the given mean and std
		''' </summary>
		''' <param name="mean"> the mean </param>
		''' <param name="std">  the standard deviation </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public NormalDistribution(@JsonProperty("mean") double mean, @JsonProperty("std") double std)
		Public Sub New(ByVal mean As Double, ByVal std As Double)
			Me.mean_Conflict = mean
			Me.std_Conflict = std
		End Sub

		Public Overridable Property Mean As Double
			Get
				Return mean_Conflict
			End Get
			Set(ByVal mean As Double)
				Me.mean_Conflict = mean
			End Set
		End Property


		Public Overridable Property Std As Double
			Get
				Return std_Conflict
			End Get
			Set(ByVal std As Double)
				Me.std_Conflict = std
			End Set
		End Property


		Public Overrides Function GetHashCode() As Integer
			Const prime As Integer = 31
			Dim result As Integer = 1
			Dim temp As Long
			temp = System.BitConverter.DoubleToInt64Bits(mean_Conflict)
			result = prime * result + CInt(temp Xor (CLng(CULng(temp) >> 32)))
			temp = System.BitConverter.DoubleToInt64Bits(std_Conflict)
			result = prime * result + CInt(temp Xor (CLng(CULng(temp) >> 32)))
			Return result
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then
				Return True
			End If
			If obj Is Nothing Then
				Return False
			End If
			If Me.GetType() <> obj.GetType() Then
				Return False
			End If
			Dim other As NormalDistribution = DirectCast(obj, NormalDistribution)
			If System.BitConverter.DoubleToInt64Bits(mean_Conflict) <> System.BitConverter.DoubleToInt64Bits(other.mean_Conflict) Then
				Return False
			End If
			If System.BitConverter.DoubleToInt64Bits(std_Conflict) <> System.BitConverter.DoubleToInt64Bits(other.std_Conflict) Then
				Return False
			End If
			Return True
		End Function

		Public Overrides Function ToString() As String
			Return "NormalDistribution(" & "mean=" & mean_Conflict & ", std=" & std_Conflict & ")"c
		End Function
	End Class

End Namespace