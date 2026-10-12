Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.deeplearning4j.nn.weights.embeddings

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class ArrayEmbeddingInitializer implements EmbeddingInitializer
	<Serializable>
	Public Class ArrayEmbeddingInitializer
		Implements EmbeddingInitializer

		Private ReadOnly embeddings As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ArrayEmbeddingInitializer(@NonNull INDArray embeddings)
		Public Sub New(ByVal embeddings As INDArray)
			Preconditions.checkState(embeddings.rank() = 2, "Embedding array must be rank 2 with shape [vocabSize, vectorSize], got array with shape %ndShape", embeddings)
			Me.embeddings = embeddings
		End Sub

		Public Overridable Sub loadWeightsInto(ByVal array As INDArray) Implements EmbeddingInitializer.loadWeightsInto
			array.assign(embeddings)
		End Sub

		Public Overridable Function vocabSize() As Long Implements EmbeddingInitializer.vocabSize
			Return embeddings.size(0)
		End Function

		Public Overridable Function vectorSize() As Integer Implements EmbeddingInitializer.vectorSize
			Return CInt(embeddings.size(1))
		End Function

		Public Overridable Function jsonSerializable() As Boolean Implements EmbeddingInitializer.jsonSerializable
			Return False
		End Function
	End Class

End Namespace