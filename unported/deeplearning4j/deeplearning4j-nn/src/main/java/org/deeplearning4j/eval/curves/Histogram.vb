Imports System
Imports Data = lombok.Data
Imports BaseHistogram = org.nd4j.evaluation.curves.BaseHistogram
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

Namespace org.deeplearning4j.eval.curves

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated @Data public class Histogram extends org.nd4j.evaluation.curves.Histogram
	<Obsolete>
	Public Class Histogram
		Inherits org.nd4j.evaluation.curves.Histogram

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.curves.Histogram"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Histogram(@JsonProperty("title") String title, @JsonProperty("lower") double lower, @JsonProperty("upper") double upper, @JsonProperty("binCounts") int[] binCounts)
		Public Sub New(ByVal title As String, ByVal lower As Double, ByVal upper As Double, ByVal binCounts() As Integer)
			MyBase.New(title, lower, upper, binCounts)
		End Sub

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.curves.Histogram"/> 
		Public Shared Function fromJson(ByVal json As String) As Histogram
			Return BaseHistogram.fromJson(json, GetType(Histogram))
		End Function

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.curves.Histogram"/> 
		Public Shared Function fromYaml(ByVal yaml As String) As Histogram
			Return BaseHistogram.fromYaml(yaml, GetType(Histogram))
		End Function
	End Class

End Namespace