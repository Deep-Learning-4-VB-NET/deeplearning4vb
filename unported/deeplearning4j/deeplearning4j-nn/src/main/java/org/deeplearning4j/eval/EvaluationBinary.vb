Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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
'ORIGINAL LINE: @Deprecated @NoArgsConstructor @EqualsAndHashCode(callSuper = true) @Data public class EvaluationBinary extends org.nd4j.evaluation.classification.EvaluationBinary implements IEvaluation<org.nd4j.evaluation.classification.EvaluationBinary>
	<Obsolete>
	Public Class EvaluationBinary
		Inherits org.nd4j.evaluation.classification.EvaluationBinary
		Implements IEvaluation(Of org.nd4j.evaluation.classification.EvaluationBinary)

		<Obsolete>
		Public Shadows Const DEFAULT_PRECISION As Integer = 4
		<Obsolete>
		Public Shadows Const DEFAULT_EDGE_VALUE As Double = 0.0

		''' <summary>
		''' Use <seealso cref="org.nd4j.evaluation.classification.EvaluationBinary"/>
		''' </summary>
		<Obsolete>
		Public Sub New(ByVal decisionThreshold As INDArray)
			MyBase.New(decisionThreshold)
		End Sub

		''' <summary>
		''' Use <seealso cref="org.nd4j.evaluation.classification.EvaluationBinary"/>
		''' </summary>
		<Obsolete>
		Public Sub New(ByVal size As Integer, ByVal rocBinarySteps As Integer?)
			MyBase.New(size, rocBinarySteps)
		End Sub

		''' <summary>
		''' Use <seealso cref="org.nd4j.evaluation.classification.EvaluationBinary.fromJson(String)"/>
		''' </summary>
		<Obsolete>
		Public Shared Function fromJson(ByVal json As String) As EvaluationBinary
			Return fromJson(json, GetType(EvaluationBinary))
		End Function

		''' <summary>
		''' Use <seealso cref="org.nd4j.evaluation.classification.EvaluationBinary.fromYaml(String)"/>
		''' </summary>
		<Obsolete>
		Public Shared Function fromYaml(ByVal yaml As String) As EvaluationBinary
			Return fromYaml(yaml, GetType(EvaluationBinary))
		End Function


	End Class

End Namespace