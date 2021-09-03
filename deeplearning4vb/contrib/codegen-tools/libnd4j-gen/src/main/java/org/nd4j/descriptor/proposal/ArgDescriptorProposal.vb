Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports OpNamespace = org.nd4j.ir.OpNamespace

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.descriptor.proposal

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder @Data public class ArgDescriptorProposal
	Public Class ArgDescriptorProposal

		Private descriptor As OpNamespace.ArgDescriptor

		Private sourceLine As String

		Private proposalWeight As Double

		Private sourceOfProposal As String
	End Class

End Namespace