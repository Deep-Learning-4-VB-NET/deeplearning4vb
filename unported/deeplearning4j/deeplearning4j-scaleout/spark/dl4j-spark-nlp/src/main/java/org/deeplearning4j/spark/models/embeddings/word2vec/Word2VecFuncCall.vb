Imports System
Imports System.Collections.Generic
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord

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


	<Obsolete, Serializable>
	Public Class Word2VecFuncCall
'JAVA TO VB CONVERTER NOTE: The field param was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private param_Conflict As Broadcast(Of Word2VecParam)
'JAVA TO VB CONVERTER NOTE: The field wordsSeen was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private wordsSeen_Conflict As Long?
'JAVA TO VB CONVERTER NOTE: The field sentence was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private sentence_Conflict As IList(Of VocabWord)

		Public Sub New(ByVal param As Broadcast(Of Word2VecParam), ByVal wordsSeen As Long?, ByVal sentence As IList(Of VocabWord))
			Me.param_Conflict = param
			Me.wordsSeen_Conflict = wordsSeen
			Me.sentence_Conflict = sentence
		End Sub

		Public Overridable Property Param As Broadcast(Of Word2VecParam)
			Get
				Return param_Conflict
			End Get
			Set(ByVal param As Broadcast(Of Word2VecParam))
				Me.param_Conflict = param
			End Set
		End Property


		Public Overridable Property WordsSeen As Long?
			Get
				Return wordsSeen_Conflict
			End Get
			Set(ByVal wordsSeen As Long?)
				Me.wordsSeen_Conflict = wordsSeen
			End Set
		End Property


		Public Overridable Property Sentence As IList(Of VocabWord)
			Get
				Return sentence_Conflict
			End Get
			Set(ByVal sentence As IList(Of VocabWord))
				Me.sentence_Conflict = sentence
			End Set
		End Property

	End Class

End Namespace