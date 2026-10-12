Imports System
Imports NonNull = lombok.NonNull
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

	<Obsolete>
	Public Class ReliabilityDiagram
		Inherits org.nd4j.evaluation.curves.ReliabilityDiagram

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""org.nd4j.evaluation.curves.ReliabilityDiagram""/>") public ReliabilityDiagram(@JsonProperty("title") String title, @NonNull @JsonProperty("meanPredictedValueX") double[] meanPredictedValueX, @NonNull @JsonProperty("fractionPositivesY") double[] fractionPositivesY)
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.curves.ReliabilityDiagram""/>")>
		Public Sub New(ByVal title As String, ByVal meanPredictedValueX() As Double, ByVal fractionPositivesY() As Double)
			MyBase.New(title, meanPredictedValueX, fractionPositivesY)
		End Sub
	End Class

End Namespace