Imports System
Imports System.Text
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor

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

Namespace org.nd4j.linalg.heartbeat.reports


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class Environment implements java.io.Serializable
	<Serializable>
	Public Class Environment
		Private serialVersionID As Long

	'    
	'        number of cores available to jvm
	'     
		Private numCores As Integer

	'    
	'        memory available within current process
	'     
		Private availableMemory As Long

	'    
	'        System.getPriority("java.specification.version");
	'     
		Private javaVersion As String


	'    
	'        System.getProperty("os.opName");
	'     
		Private osName As String

	'    
	'        System.getProperty("os.arch");
	'        however, in 97% of cases it will be amd64. it will be better to have JNI call for that in future
	'     
		Private osArch As String

	'    
	'        Nd4j backend being used within current JVM session
	'    
		Private backendUsed As String


		Public Overridable Function toCompactString() As String
			Dim builder As New StringBuilder()

	'        
	'         new format is:
	'         Backend ( cores, ram, jvm, Linux, arch)
	'         
	'        
	'            builder.append(numCores).append("cores/");
	'            builder.append(availableMemory / 1024 / 1024 / 1024).append("GB/");
	'            builder.append("jvm").append(javaVersion).append("/");
	'            builder.append(osName).append("/");
	'            builder.append(osArch).append("/");
	'            builder.append(backendUsed).append(" ");
	'        

			builder.Append(backendUsed).Append(" (").Append(numCores).Append(" cores ").Append(Math.Max(availableMemory \ 1024 \ 1024 \ 1024, 1)).Append("GB ").Append(osName).Append(" ").Append(osArch).Append(")")

			Return builder.ToString()
		End Function
	End Class

End Namespace