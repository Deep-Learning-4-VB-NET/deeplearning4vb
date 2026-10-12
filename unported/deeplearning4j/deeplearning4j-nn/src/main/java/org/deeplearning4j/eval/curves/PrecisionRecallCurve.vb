Imports System
Imports Preconditions = org.nd4j.shade.guava.base.Preconditions
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
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
'ORIGINAL LINE: @Deprecated @Data @EqualsAndHashCode(callSuper = true) public class PrecisionRecallCurve extends org.nd4j.evaluation.curves.PrecisionRecallCurve
	<Obsolete>
	Public Class PrecisionRecallCurve
		Inherits org.nd4j.evaluation.curves.PrecisionRecallCurve

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""org.nd4j.evaluation.curves.ReliabilityDiagram""/>") public PrecisionRecallCurve(@JsonProperty("threshold") double[] threshold, @JsonProperty("precision") double[] precision, @JsonProperty("recall") double[] recall, @JsonProperty("tpCount") int[] tpCount, @JsonProperty("fpCount") int[] fpCount, @JsonProperty("fnCount") int[] fnCount, @JsonProperty("totalCount") int totalCount)
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.curves.ReliabilityDiagram""/>")>
		Public Sub New(ByVal threshold() As Double, ByVal precision() As Double, ByVal recall() As Double, ByVal tpCount() As Integer, ByVal fpCount() As Integer, ByVal fnCount() As Integer, ByVal totalCount As Integer)
			MyBase.New(threshold, precision, recall, tpCount, fpCount, fnCount, totalCount)
		End Sub

		Public Class Point
			Inherits org.nd4j.evaluation.curves.PrecisionRecallCurve.Point

			Public Sub New(ByVal idx As Integer, ByVal threshold As Double, ByVal precision As Double, ByVal recall As Double)
				MyBase.New(idx, threshold, precision, recall)
			End Sub
		End Class

		Public Class Confusion
			Inherits org.nd4j.evaluation.curves.PrecisionRecallCurve.Confusion

			Public Sub New(ByVal point As org.nd4j.evaluation.curves.PrecisionRecallCurve.Point, ByVal tpCount As Integer, ByVal fpCount As Integer, ByVal fnCount As Integer, ByVal tnCount As Integer)
				MyBase.New(point, tpCount, fpCount, fnCount, tnCount)
			End Sub
		End Class
	End Class

End Namespace