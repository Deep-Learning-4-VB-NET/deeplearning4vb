Imports System
Imports System.Collections.Generic
Imports SparkConf = org.apache.spark.SparkConf

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
	''' @author jeffreytang
	''' </summary>
	<Obsolete>
	Public Class Word2VecVariables

		Public Const NAME_SPACE As String = "org.deeplearning4j.scaleout.perform.models.word2vec"
		Public Shared ReadOnly VECTOR_LENGTH As String = NAME_SPACE & ".length"
		Public Shared ReadOnly ADAGRAD As String = NAME_SPACE & ".adagrad"
		Public Shared ReadOnly NEGATIVE As String = NAME_SPACE & ".negative"
		Public Shared ReadOnly NUM_WORDS As String = NAME_SPACE & ".numwords"
		Public Shared ReadOnly TABLE As String = NAME_SPACE & ".table"
		Public Shared ReadOnly WINDOW As String = NAME_SPACE & ".window"
		Public Shared ReadOnly ALPHA As String = NAME_SPACE & ".alpha"
		Public Shared ReadOnly MIN_ALPHA As String = NAME_SPACE & ".minalpha"
		Public Shared ReadOnly ITERATIONS As String = NAME_SPACE & ".iterations"
		Public Shared ReadOnly N_GRAMS As String = NAME_SPACE & ".ngrams"
		Public Shared ReadOnly TOKENIZER As String = NAME_SPACE & ".tokenizer"
		Public Shared ReadOnly TOKEN_PREPROCESSOR As String = NAME_SPACE & ".preprocessor"
		Public Shared ReadOnly REMOVE_STOPWORDS As String = NAME_SPACE & ".removestopwords"
		Public Shared ReadOnly SEED As String = NAME_SPACE & ".SEED"

		Public Shared ReadOnly defaultVals As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass()

		Private Class HashMapAnonymousInnerClass
			Inherits Dictionary(Of String, Object)

			Public Sub New()

				Me.put(VECTOR_LENGTH, 100)
				Me.put(ADAGRAD, False)
				Me.put(NEGATIVE, 5)
				Me.put(NUM_WORDS, 1)
				' TABLE would be a string of byte of the ndarray used for -ve sampling
				Me.put(WINDOW, 5)
				Me.put(ALPHA, 0.025)
				Me.put(MIN_ALPHA, 1e-2)
				Me.put(ITERATIONS, 1)
				Me.put(N_GRAMS, 1)
				Me.put(TOKENIZER, "org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory")
				Me.put(TOKEN_PREPROCESSOR, "org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor")
				Me.put(REMOVE_STOPWORDS, False)
				Me.put(SEED, 42L)
			End Sub

		End Class

		Private Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T getDefault(String variableName)
		Public Shared Function getDefault(Of T)(ByVal variableName As String) As T
			Return DirectCast(defaultVals(variableName), T)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T assignVar(String variableName, org.apache.spark.SparkConf conf, @Class clazz) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function assignVar(Of T)(ByVal variableName As String, ByVal conf As SparkConf, ByVal clazz As Type) As T
			Dim ret As Object
			If clazz.Equals(GetType(Integer)) Then
				ret = conf.getInt(variableName, CType(getDefault(variableName), Integer?))

			ElseIf clazz.Equals(GetType(Double)) Then
				ret = conf.getDouble(variableName, CType(getDefault(variableName), Double?))

			ElseIf clazz.Equals(GetType(Boolean)) Then
				ret = conf.getBoolean(variableName, CType(getDefault(variableName), Boolean?))

			ElseIf clazz.Equals(GetType(String)) Then
				ret = conf.get(variableName, CStr(getDefault(variableName)))

			ElseIf clazz.Equals(GetType(Long)) Then
				ret = conf.getLong(variableName, CType(getDefault(variableName), Long?))
			Else
				Throw New Exception("Variable Type not supported. Only boolean, int, double and String supported.")
			End If
			Return DirectCast(ret, T)
		End Function
	End Class

End Namespace