Imports System.Collections.Generic
Imports System.IO
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.nd4j.imports.descriptors.onnx


	Public Class OnnxDescriptorParser


		''' <summary>
		''' Get the onnx op descriptors by name </summary>
		''' <returns> the onnx op descriptors by name </returns>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.util.Map<String,OpDescriptor> onnxOpDescriptors() throws Exception
		Public Shared Function onnxOpDescriptors() As IDictionary(Of String, OpDescriptor)
			Using [is] As Stream = (New org.nd4j.common.io.ClassPathResource("onnxops.json")).InputStream
				Dim objectMapper As New ObjectMapper()
				Dim opDescriptor As OnnxDescriptor = objectMapper.readValue([is],GetType(OnnxDescriptor))
				Dim descriptorMap As IDictionary(Of String, OpDescriptor) = New Dictionary(Of String, OpDescriptor)()
				For Each descriptor As OpDescriptor In opDescriptor.getDescriptors()
					descriptorMap(descriptor.getName()) = descriptor
				Next descriptor



				Return descriptorMap
			End Using
		End Function


	End Class

End Namespace