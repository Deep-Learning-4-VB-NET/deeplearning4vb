Imports System
Imports Preconditions = org.nd4j.shade.guava.base.Preconditions
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
'ORIGINAL LINE: @Deprecated @Data @EqualsAndHashCode(exclude = {"auc"}, callSuper = false) public class RocCurve extends org.nd4j.evaluation.curves.RocCurve
	<Obsolete>
	Public Class RocCurve
		Inherits org.nd4j.evaluation.curves.RocCurve

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""org.nd4j.evaluation.curves.RocCurve""/>") public RocCurve(@JsonProperty("threshold") double[] threshold, @JsonProperty("fpr") double[] fpr, @JsonProperty("tpr") double[] tpr)
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.curves.RocCurve""/>")>
		Public Sub New(ByVal threshold() As Double, ByVal fpr() As Double, ByVal tpr() As Double)
			MyBase.New(threshold, fpr, tpr)
		End Sub


		''' @deprecated Use <seealso cref="org.nd4j.evaluation.curves.RocCurve"/> 
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.curves.RocCurve""/>")>
		Public Shared Function fromJson(ByVal json As String) As RocCurve
			Return fromJson(json, GetType(RocCurve))
		End Function

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.curves.RocCurve"/> 
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.curves.RocCurve""/>")>
		Public Shared Function fromYaml(ByVal yaml As String) As RocCurve
			Return fromYaml(yaml, GetType(RocCurve))
		End Function

	End Class

End Namespace