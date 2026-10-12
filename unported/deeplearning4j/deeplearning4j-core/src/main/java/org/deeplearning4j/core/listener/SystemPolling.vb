Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports YAMLFactory = org.nd4j.shade.jackson.dataformat.yaml.YAMLFactory
Imports SystemInfo = oshi.json.SystemInfo

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

Namespace org.deeplearning4j.core.listener


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SystemPolling
	Public Class SystemPolling

		Private scheduledExecutorService As ScheduledExecutorService
		Private pollEveryMillis As Long
		Private outputDirectory As File
		Private nameProvider As NameProvider
		Private objectMapper As New ObjectMapper(New YAMLFactory())

		Private Sub New(ByVal pollEveryMillis As Long, ByVal outputDirectory As File, ByVal nameProvider As NameProvider)
			Me.pollEveryMillis = pollEveryMillis
			Me.outputDirectory = outputDirectory
			Me.nameProvider = nameProvider
		End Sub


		''' <summary>
		''' Run the polling in the background as a thread pool
		''' running every <seealso cref="pollEveryMillis"/> milliseconds
		''' </summary>
		Public Overridable Sub run()
			scheduledExecutorService = Executors.newScheduledThreadPool(1)
			scheduledExecutorService.scheduleAtFixedRate(Sub()
			Dim systemInfo As New SystemInfo()
			Dim hardwareMetric_Conflict As HardwareMetric = HardwareMetric.fromSystem(systemInfo,nameProvider.nextName())
			Dim hardwareFile As New File(outputDirectory,hardwareMetric_Conflict.getName() & ".yml")
			Try
				objectMapper.writeValue(hardwareFile,hardwareMetric_Conflict)
			Catch e As IOException
				log.error("",e)
			End Try
			End Sub,0,pollEveryMillis, TimeUnit.MILLISECONDS)
		End Sub


		''' <summary>
		''' Shut down the background polling
		''' </summary>
		Public Overridable Sub stopPolling()
			scheduledExecutorService.shutdownNow()
		End Sub


		''' <summary>
		''' The naming sequence provider.
		''' This is for allowing generation of naming the output
		''' according to some semantic sequence (such as a neural net epoch
		''' or some time stamp)
		''' </summary>
		Public Interface NameProvider
			Function nextName() As String
		End Interface

		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field pollEveryMillis was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend pollEveryMillis_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field outputDirectory was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend outputDirectory_Conflict As File

'JAVA TO VB CONVERTER NOTE: The field nameProvider was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nameProvider_Conflict As NameProvider = New NameProviderAnonymousInnerClass()

			Private Class NameProviderAnonymousInnerClass
				Implements NameProvider

				Public Function nextName() As String Implements NameProvider.nextName
					Return System.Guid.randomUUID().ToString()
				End Function
			End Class


			''' <summary>
			''' The name provider for  this <seealso cref="SystemPolling"/>
			''' the default value for this is a simple UUID </summary>
			''' <param name="nameProvider"> the name provider to use
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter nameProvider was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nameProvider(ByVal nameProvider_Conflict As NameProvider) As Builder
				Me.nameProvider_Conflict = nameProvider_Conflict
				Return Me
			End Function


			''' <summary>
			''' The interval in milliseconds in which to poll
			''' the system for diagnostics </summary>
			''' <param name="pollEveryMillis"> the interval in milliseconds
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter pollEveryMillis was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function pollEveryMillis(ByVal pollEveryMillis_Conflict As Long) As Builder
				Me.pollEveryMillis_Conflict = pollEveryMillis_Conflict
				Return Me
			End Function

			''' <summary>
			''' The output directory for the files </summary>
			''' <param name="outputDirectory"> the output directory for the logs
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter outputDirectory was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function outputDirectory(ByVal outputDirectory_Conflict As File) As Builder
				Me.outputDirectory_Conflict = outputDirectory_Conflict
				Return Me
			End Function

			Public Overridable Function build() As SystemPolling
				Return New SystemPolling(pollEveryMillis_Conflict,outputDirectory_Conflict,nameProvider_Conflict)
			End Function

		End Class

	End Class

End Namespace