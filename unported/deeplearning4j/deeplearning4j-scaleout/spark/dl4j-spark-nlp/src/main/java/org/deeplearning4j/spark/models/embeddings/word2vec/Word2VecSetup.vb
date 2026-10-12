Imports System
Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports Tuple2 = scala.Tuple2

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


	<Obsolete>
	Public Class Word2VecSetup
		Implements [Function](Of Tuple2(Of IList(Of VocabWord), Long), Word2VecFuncCall)

		Private param As Broadcast(Of Word2VecParam)

		Public Sub New(ByVal param As Broadcast(Of Word2VecParam))
			Me.param = param
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Word2VecFuncCall call(scala.Tuple2<java.util.List<org.deeplearning4j.models.word2vec.VocabWord>, Long> listLongTuple2) throws Exception
		Public Overrides Function [call](ByVal listLongTuple2 As Tuple2(Of IList(Of VocabWord), Long)) As Word2VecFuncCall
			Return New Word2VecFuncCall(param, listLongTuple2._2(), listLongTuple2._1())
		End Function
	End Class

End Namespace