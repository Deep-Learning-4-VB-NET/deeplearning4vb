Imports System
Imports lombok
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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
'ORIGINAL LINE: @Data public class EnvironmentVarPortSupplier implements org.nd4j.parameterserver.distributed.v2.transport.PortSupplier
	<Serializable>
	Public Class EnvironmentVarPortSupplier
		Implements PortSupplier

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) private int port = -1;
'JAVA TO VB CONVERTER NOTE: The field port was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private port_Conflict As Integer = -1

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) private String variableName;
		Private variableName As String

		Protected Friend Sub New()
			'
		End Sub

		''' <summary>
		''' This constructor builds StaticPortSupplier instance with pre-defined port </summary>
		''' <param name="port"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EnvironmentVarPortSupplier(@NonNull String varName)
		Public Sub New(ByVal varName As String)
			variableName = varName
		End Sub

		Public Overridable ReadOnly Property Port As Integer Implements PortSupplier.getPort
			Get
					Dim variable As val = Environment.GetEnvironmentVariable(variableName)
					If variable Is Nothing Then
						Throw New ND4JIllegalStateException("Unable to get networking port from environment variable:" & " environment variable [" & variableName & "] isn't defined")
					End If
    
					Try
						port_Conflict = Convert.ToInt32(variable)
					Catch e As System.FormatException
						Throw New ND4JIllegalStateException("Unable to get network port from environment variable:" & " environment variable [" & variableName & "] contains bad value: [" & variable & "]")
					End Try
    
					Preconditions.checkState(port_Conflict > 0 AndAlso port_Conflict <= 65535, "Invalid port for environment variable: ports must be" & "between 0 (exclusive) and 65535 (inclusive). Got port: %s", port_Conflict)
    
				Return port_Conflict
			End Get
		End Property
	End Class

End Namespace