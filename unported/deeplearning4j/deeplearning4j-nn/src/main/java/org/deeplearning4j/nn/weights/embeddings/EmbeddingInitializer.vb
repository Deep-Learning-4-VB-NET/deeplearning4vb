Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface EmbeddingInitializer extends java.io.Serializable
	Public Interface EmbeddingInitializer

		''' <summary>
		''' Load the weights into the specified INDArray </summary>
		''' <param name="array"> Array of shape [vocabSize, vectorSize] </param>
		Sub loadWeightsInto(ByVal array As INDArray)

		''' <returns> Size of the vocabulary </returns>
		Function vocabSize() As Long

		''' <returns> Size of each vector </returns>
		Function vectorSize() As Integer

		''' <returns> True if the embedding initializer can be safely serialized as JSON </returns>
		Function jsonSerializable() As Boolean
	End Interface

End Namespace