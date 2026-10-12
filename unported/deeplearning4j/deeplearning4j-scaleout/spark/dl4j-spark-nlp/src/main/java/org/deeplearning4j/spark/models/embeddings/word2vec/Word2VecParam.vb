Imports System
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports org.deeplearning4j.models.embeddings.inmemory
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.spark.models.embeddings.word2vec


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	<Obsolete, Serializable>
	Public Class Word2VecParam

'JAVA TO VB CONVERTER NOTE: The field useAdaGrad was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private useAdaGrad_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field negative was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private negative_Conflict As Double = 5
'JAVA TO VB CONVERTER NOTE: The field numWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numWords_Conflict As Integer = 1
'JAVA TO VB CONVERTER NOTE: The field table was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private table_Conflict As INDArray
'JAVA TO VB CONVERTER NOTE: The field window was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private window_Conflict As Integer = 5
'JAVA TO VB CONVERTER NOTE: The field nextRandom was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private nextRandom_Conflict As New AtomicLong(5)
'JAVA TO VB CONVERTER NOTE: The field alpha was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private alpha_Conflict As Double = 0.025
'JAVA TO VB CONVERTER NOTE: The field minAlpha was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private minAlpha_Conflict As Double = 1e-2
'JAVA TO VB CONVERTER NOTE: The field totalWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private totalWords_Conflict As Integer = 1
		Private transient As static
'JAVA TO VB CONVERTER NOTE: The field lastChecked was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private lastChecked_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field wordCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private wordCount_Conflict As Broadcast(Of AtomicLong)
'JAVA TO VB CONVERTER NOTE: The field weights was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private weights_Conflict As InMemoryLookupTable
'JAVA TO VB CONVERTER NOTE: The field vectorLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private vectorLength_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field expTable was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private expTable_Conflict As Broadcast(Of Double())
'JAVA TO VB CONVERTER NOTE: The field wordsSeen was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private wordsSeen_Conflict As New AtomicLong(0)
'JAVA TO VB CONVERTER NOTE: The field lastWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private lastWords_Conflict As New AtomicLong(0)

		Public Sub New(ByVal useAdaGrad As Boolean, ByVal negative As Double, ByVal numWords As Integer, ByVal table As INDArray, ByVal window As Integer, ByVal nextRandom As AtomicLong, ByVal alpha As Double, ByVal minAlpha As Double, ByVal totalWords As Integer, ByVal lastChecked As Integer, ByVal wordCount As Broadcast(Of AtomicLong), ByVal weights As InMemoryLookupTable, ByVal vectorLength As Integer, ByVal expTable As Broadcast(Of Double()))
			Me.useAdaGrad_Conflict = useAdaGrad
			Me.negative_Conflict = negative
			Me.numWords_Conflict = numWords
			Me.table_Conflict = table
			Me.window_Conflict = window
			Me.nextRandom_Conflict = nextRandom
			Me.alpha_Conflict = alpha
			Me.minAlpha_Conflict = minAlpha
			Me.totalWords_Conflict = totalWords
			Me.lastChecked_Conflict = lastChecked
			Me.wordCount_Conflict = wordCount
			Me.weights_Conflict = weights
			Me.vectorLength_Conflict = vectorLength
			Me.expTable_Conflict = expTable
		End Sub

		Public Overridable Property LastWords As AtomicLong
			Get
				Return lastWords_Conflict
			End Get
			Set(ByVal lastWords As AtomicLong)
				Me.lastWords_Conflict = lastWords
			End Set
		End Property


		Public Overridable Property WordsSeen As AtomicLong
			Get
				Return wordsSeen_Conflict
			End Get
			Set(ByVal wordsSeen As AtomicLong)
				Me.wordsSeen_Conflict = wordsSeen
			End Set
		End Property


		Public Overridable Property ExpTable As Broadcast(Of Double())
			Get
				Return expTable_Conflict
			End Get
			Set(ByVal expTable As Broadcast(Of Double()))
				Me.expTable_Conflict = expTable
			End Set
		End Property


		Public Overridable Property UseAdaGrad As Boolean
			Get
				Return useAdaGrad_Conflict
			End Get
			Set(ByVal useAdaGrad As Boolean)
				Me.useAdaGrad_Conflict = useAdaGrad
			End Set
		End Property


		Public Overridable Property Negative As Double
			Get
				Return negative_Conflict
			End Get
			Set(ByVal negative As Double)
				Me.negative_Conflict = negative
			End Set
		End Property


		Public Overridable Property NumWords As Integer
			Get
				Return numWords_Conflict
			End Get
			Set(ByVal numWords As Integer)
				Me.numWords_Conflict = numWords
			End Set
		End Property


		Public Overridable Property Table As INDArray
			Get
				Return table_Conflict
			End Get
			Set(ByVal table As INDArray)
				Me.table_Conflict = table
			End Set
		End Property


		Public Overridable Property Window As Integer
			Get
				Return window_Conflict
			End Get
			Set(ByVal window As Integer)
				Me.window_Conflict = window
			End Set
		End Property


		Public Overridable Property NextRandom As AtomicLong
			Get
				Return nextRandom_Conflict
			End Get
			Set(ByVal nextRandom As AtomicLong)
				Me.nextRandom_Conflict = nextRandom
			End Set
		End Property


		Public Overridable Property Alpha As Double
			Get
				Return alpha_Conflict
			End Get
			Set(ByVal alpha As Double)
				Me.alpha_Conflict = alpha
			End Set
		End Property


		Public Overridable Property MinAlpha As Double
			Get
				Return minAlpha_Conflict
			End Get
			Set(ByVal minAlpha As Double)
				Me.minAlpha_Conflict = minAlpha
			End Set
		End Property


		Public Overridable Property TotalWords As Integer
			Get
				Return totalWords_Conflict
			End Get
			Set(ByVal totalWords As Integer)
				Me.totalWords_Conflict = totalWords
			End Set
		End Property


		Public Shared ReadOnly Property Log As Logger
			Get
				Return log
			End Get
		End Property

		Public Overridable Property LastChecked As Integer
			Get
				Return lastChecked_Conflict
			End Get
			Set(ByVal lastChecked As Integer)
				Me.lastChecked_Conflict = lastChecked
			End Set
		End Property


		Public Overridable Property WordCount As Broadcast(Of AtomicLong)
			Get
				Return wordCount_Conflict
			End Get
			Set(ByVal wordCount As Broadcast(Of AtomicLong))
				Me.wordCount_Conflict = wordCount
			End Set
		End Property


		Public Overridable Property Weights As InMemoryLookupTable
			Get
				Return weights_Conflict
			End Get
			Set(ByVal weights As InMemoryLookupTable)
				Me.weights_Conflict = weights
			End Set
		End Property




		Public Overridable Property VectorLength As Integer
			Get
				Return vectorLength_Conflict
			End Get
			Set(ByVal vectorLength As Integer)
				Me.vectorLength_Conflict = vectorLength
			End Set
		End Property


		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field useAdaGrad was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend useAdaGrad_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field negative was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend negative_Conflict As Double = 0
'JAVA TO VB CONVERTER NOTE: The field numWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend numWords_Conflict As Integer = 1
'JAVA TO VB CONVERTER NOTE: The field table was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend table_Conflict As INDArray
'JAVA TO VB CONVERTER NOTE: The field window was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend window_Conflict As Integer = 5
'JAVA TO VB CONVERTER NOTE: The field nextRandom was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nextRandom_Conflict As AtomicLong
'JAVA TO VB CONVERTER NOTE: The field alpha was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend alpha_Conflict As Double = 0.025
'JAVA TO VB CONVERTER NOTE: The field minAlpha was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend minAlpha_Conflict As Double = 0.01
'JAVA TO VB CONVERTER NOTE: The field totalWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend totalWords_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field lastChecked was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend lastChecked_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field wordCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend wordCount_Conflict As Broadcast(Of AtomicLong)
'JAVA TO VB CONVERTER NOTE: The field weights was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend weights_Conflict As InMemoryLookupTable
'JAVA TO VB CONVERTER NOTE: The field vectorLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend vectorLength_Conflict As Integer = 300
'JAVA TO VB CONVERTER NOTE: The field expTable was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend expTable_Conflict As Broadcast(Of Double())

'JAVA TO VB CONVERTER NOTE: The parameter expTable was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function expTable(ByVal expTable_Conflict As Broadcast(Of Double())) As Builder
				Me.expTable_Conflict = expTable_Conflict
				Return Me
			End Function


'JAVA TO VB CONVERTER NOTE: The parameter useAdaGrad was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function useAdaGrad(ByVal useAdaGrad_Conflict As Boolean) As Builder
				Me.useAdaGrad_Conflict = useAdaGrad_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter negative was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function negative(ByVal negative_Conflict As Double) As Builder
				Me.negative_Conflict = negative_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter numWords was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function numWords(ByVal numWords_Conflict As Integer) As Builder
				Me.numWords_Conflict = numWords_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter table was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function table(ByVal table_Conflict As INDArray) As Builder
				Me.table_Conflict = table_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter window was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function window(ByVal window_Conflict As Integer) As Builder
				Me.window_Conflict = window_Conflict
				Return Me
			End Function

			Public Overridable Function setNextRandom(ByVal nextRandom As AtomicLong) As Builder
				Me.nextRandom_Conflict = nextRandom
				Return Me
			End Function

			Public Overridable Function setAlpha(ByVal alpha As Double) As Builder
				Me.alpha_Conflict = alpha
				Return Me
			End Function

			Public Overridable Function setMinAlpha(ByVal minAlpha As Double) As Builder
				Me.minAlpha_Conflict = minAlpha
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter totalWords was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function totalWords(ByVal totalWords_Conflict As Integer) As Builder
				Me.totalWords_Conflict = totalWords_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter lastChecked was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lastChecked(ByVal lastChecked_Conflict As Integer) As Builder
				Me.lastChecked_Conflict = lastChecked_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter wordCount was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function wordCount(ByVal wordCount_Conflict As Broadcast(Of AtomicLong)) As Builder
				Me.wordCount_Conflict = wordCount_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter weights was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weights(ByVal weights_Conflict As InMemoryLookupTable) As Builder
				Me.weights_Conflict = weights_Conflict
				Return Me
			End Function

			Public Overridable Function setVectorLength(ByVal vectorLength As Integer) As Builder
				Me.vectorLength_Conflict = vectorLength
				Return Me
			End Function

			Public Overridable Function build() As Word2VecParam
				Return New Word2VecParam(useAdaGrad_Conflict, negative_Conflict, numWords_Conflict, table_Conflict, window_Conflict, nextRandom_Conflict, alpha_Conflict, minAlpha_Conflict, totalWords_Conflict, lastChecked_Conflict, wordCount_Conflict, weights_Conflict, vectorLength_Conflict, expTable_Conflict)
			End Function
		End Class
	End Class

End Namespace