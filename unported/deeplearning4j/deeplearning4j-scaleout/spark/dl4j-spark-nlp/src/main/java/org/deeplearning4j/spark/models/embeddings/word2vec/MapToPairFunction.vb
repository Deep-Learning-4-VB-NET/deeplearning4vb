Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives

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
	Public Class MapToPairFunction
		Implements [Function](Of KeyValuePair(Of VocabWord, INDArray), Pair(Of VocabWord, INDArray))

		Public Overrides Function [call](ByVal pair As KeyValuePair(Of VocabWord, INDArray)) As Pair(Of VocabWord, INDArray)
			Return New Pair(Of VocabWord, INDArray)(pair.Key, pair.Value)
		End Function
	End Class

End Namespace