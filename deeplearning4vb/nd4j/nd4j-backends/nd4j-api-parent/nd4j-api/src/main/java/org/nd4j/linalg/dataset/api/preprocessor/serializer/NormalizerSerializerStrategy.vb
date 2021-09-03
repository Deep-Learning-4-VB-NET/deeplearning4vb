Imports System.IO
Imports org.nd4j.linalg.dataset.api.preprocessor

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

Namespace org.nd4j.linalg.dataset.api.preprocessor.serializer


	Public Interface NormalizerSerializerStrategy(Of T As org.nd4j.linalg.dataset.api.preprocessor.Normalizer)
		''' <summary>
		''' Serialize a normalizer to a output stream
		''' </summary>
		''' <param name="normalizer"> the normalizer </param>
		''' <param name="stream">     the output stream to write to </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void write(T normalizer, java.io.OutputStream stream) throws java.io.IOException;
		Sub write(ByVal normalizer As T, ByVal stream As Stream)

		''' <summary>
		''' Restore a normalizer that was previously serialized by this strategy
		''' </summary>
		''' <param name="stream"> the stream to read serialized data from </param>
		''' <returns> the restored normalizer </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: T restore(java.io.InputStream stream) throws java.io.IOException;
		Function restore(ByVal stream As Stream) As T

		''' <summary>
		''' Get the enum opType of the supported normalizer
		''' </summary>
		''' <seealso cref= Normalizer#getType()
		''' </seealso>
		''' <returns> the enum opType </returns>
		ReadOnly Property SupportedType As NormalizerType
	End Interface

End Namespace