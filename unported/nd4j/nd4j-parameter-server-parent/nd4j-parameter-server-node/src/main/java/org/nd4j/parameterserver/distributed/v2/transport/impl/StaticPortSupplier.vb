Imports System
Imports lombok
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PortSupplier = org.nd4j.parameterserver.distributed.v2.transport.PortSupplier

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

Namespace org.nd4j.parameterserver.distributed.v2.transport.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class StaticPortSupplier implements org.nd4j.parameterserver.distributed.v2.transport.PortSupplier
	<Serializable>
	Public Class StaticPortSupplier
		Implements PortSupplier

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) private int port = 49876;
'JAVA TO VB CONVERTER NOTE: The field port was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private port_Conflict As Integer = 49876

		Protected Friend Sub New()
			'
		End Sub

		''' <summary>
		''' This constructor builds StaticPortSupplier instance with pre-defined port </summary>
		''' <param name="port"> </param>
		Public Sub New(ByVal port As Integer)
			Preconditions.checkArgument(port > 0 AndAlso port <= 65535, "Invalid port: must be in range 1 to 65535 inclusive. Got: %s", port)
		End Sub

		Public Overridable ReadOnly Property Port As Integer Implements PortSupplier.getPort
			Get
				Return port_Conflict
			End Get
		End Property
	End Class

End Namespace