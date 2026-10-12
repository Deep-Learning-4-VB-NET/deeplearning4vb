Imports System
Imports MultiDataSetWrapperIterator = org.deeplearning4j.datasets.iterator.MultiDataSetWrapperIterator
Imports org.deeplearning4j.earlystopping.scorecalc
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports org.nd4j.evaluation
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

Namespace org.deeplearning4j.earlystopping.scorecalc.base

	Public MustInherit Class BaseIEvaluationScoreCalculator(Of T As org.deeplearning4j.nn.api.Model, U As org.nd4j.evaluation.IEvaluation)
		Implements ScoreCalculator(Of T)

		Public MustOverride Function minimizeScore() As Boolean Implements ScoreCalculator(Of T).minimizeScore

		Protected Friend iterator As MultiDataSetIterator
		Protected Friend iter As DataSetIterator

		Protected Friend Sub New(ByVal iterator As MultiDataSetIterator)
			Me.iterator = iterator
		End Sub

		Protected Friend Sub New(ByVal iterator As DataSetIterator)
			Me.iter = iterator
		End Sub

		Public Overridable Function calculateScore(ByVal network As T) As Double Implements ScoreCalculator(Of T).calculateScore
			Dim eval As U = newEval()

			If TypeOf network Is MultiLayerNetwork Then
				Dim i As DataSetIterator = (If(iter IsNot Nothing, iter, New MultiDataSetWrapperIterator(iterator)))
				eval = CType(network, MultiLayerNetwork).doEvaluation(i, eval)(0)
			ElseIf TypeOf network Is ComputationGraph Then
				Dim i As MultiDataSetIterator = (If(iterator IsNot Nothing, iterator, New MultiDataSetIteratorAdapter(iter)))
				eval = CType(network, ComputationGraph).doEvaluation(i, eval)(0)
			Else
				Throw New Exception("Unknown model type: " & network.GetType())
			End If
			Return finalScore(eval)
		End Function

		Protected Friend MustOverride Function newEval() As U

		Protected Friend MustOverride Function finalScore(ByVal eval As U) As Double


	End Class

End Namespace