Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports org.nd4j.evaluation

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

Namespace org.deeplearning4j.spark.impl.multilayer.evaluation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class IEvaluationReduceFunction<T extends org.nd4j.evaluation.IEvaluation> implements org.apache.spark.api.java.function.Function2<T[], T[], T[]>
	Public Class IEvaluationReduceFunction(Of T As org.nd4j.evaluation.IEvaluation)
		Implements Function2(Of T(), T(), T())

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public T[] call(T[] eval1, T[] eval2) throws Exception
		Public Overrides Function [call](ByVal eval1() As T, ByVal eval2() As T) As T()
			'Shouldn't *usually* happen...
			If eval1 Is Nothing Then
				Return eval2
			ElseIf eval2 Is Nothing Then
				Return eval1
			End If


			For i As Integer = 0 To eval1.Length - 1
				If eval1(i) Is Nothing Then
					eval1(i) = eval2(i)
				ElseIf eval2(i) IsNot Nothing Then
					eval1(i).merge(eval2(i))
				End If
			Next i
			Return eval1
		End Function
	End Class

End Namespace