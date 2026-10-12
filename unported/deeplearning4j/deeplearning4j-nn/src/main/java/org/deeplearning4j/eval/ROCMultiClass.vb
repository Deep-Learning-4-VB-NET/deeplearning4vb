Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode

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

Namespace org.deeplearning4j.eval

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated @Data @EqualsAndHashCode(callSuper = true) public class ROCMultiClass extends org.nd4j.evaluation.classification.ROCMultiClass implements IEvaluation<org.nd4j.evaluation.classification.ROCMultiClass>
	<Obsolete>
	Public Class ROCMultiClass
		Inherits org.nd4j.evaluation.classification.ROCMultiClass
		Implements IEvaluation(Of org.nd4j.evaluation.classification.ROCMultiClass)

		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ROCMultiClass""/>")>
		Public Shadows Const DEFAULT_STATS_PRECISION As Integer = 4

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.classification.ROCMultiClass"/> 
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ROCMultiClass""/>")>
		Public Sub New()
		End Sub
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ROCMultiClass""/>")>
		Public Sub New(ByVal thresholdSteps As Integer)
			MyBase.New(thresholdSteps)
		End Sub

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.classification.ROCMultiClass"/> 
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ROCMultiClass""/>")>
		Public Sub New(ByVal thresholdSteps As Integer, ByVal rocRemoveRedundantPts As Boolean)
			MyBase.New(thresholdSteps, rocRemoveRedundantPts)
		End Sub
	End Class

End Namespace