Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor

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
'ORIGINAL LINE: @Deprecated @EqualsAndHashCode(callSuper = true) @Data public class ROC extends org.nd4j.evaluation.classification.ROC implements IEvaluation<org.nd4j.evaluation.classification.ROC>
	<Obsolete>
	Public Class ROC
		Inherits org.nd4j.evaluation.classification.ROC
		Implements IEvaluation(Of org.nd4j.evaluation.classification.ROC)

		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ROC""/>")>
		Public Sub New()
		End Sub
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ROC""/>")>
		Public Sub New(ByVal thresholdSteps As Integer)
			MyBase.New(thresholdSteps)
		End Sub

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.classification.ROC"/> 
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ROC""/>")>
		Public Sub New(ByVal thresholdSteps As Integer, ByVal rocRemoveRedundantPts As Boolean)
			MyBase.New(thresholdSteps, rocRemoveRedundantPts)
		End Sub

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.classification.ROC"/> 
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ROC""/>")>
		Public Sub New(ByVal thresholdSteps As Integer, ByVal rocRemoveRedundantPts As Boolean, ByVal exactAllocBlockSize As Integer)
			MyBase.New(thresholdSteps, rocRemoveRedundantPts, exactAllocBlockSize)
		End Sub

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.classification.ROC.CountsForThreshold"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""org.nd4j.evaluation.classification.ROC.CountsForThreshold""/>") @NoArgsConstructor public static class CountsForThreshold extends org.nd4j.evaluation.classification.ROC.CountsForThreshold
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ROC.CountsForThreshold""/>")>
		Public Class CountsForThreshold
			Inherits org.nd4j.evaluation.classification.ROC.CountsForThreshold

			Public Sub New(ByVal threshold As Double)
				MyBase.New(threshold)
			End Sub

			Public Sub New(ByVal threshold As Double, ByVal countTruePositive As Long, ByVal countFalsePositive As Long)
				MyBase.New(threshold, countTruePositive, countFalsePositive)
			End Sub

			Public Overrides Function clone() As CountsForThreshold
				Return New CountsForThreshold(getThreshold(), getCountTruePositive(), getCountFalsePositive())
			End Function
		End Class
	End Class

End Namespace