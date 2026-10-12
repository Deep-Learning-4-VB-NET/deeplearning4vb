Imports System
Imports System.Collections.Generic
Imports com.beust.jcommander
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports [Namespace] = org.nd4j.codegen.Namespace
Imports NamespaceOps = org.nd4j.codegen.api.NamespaceOps
Imports DocsGenerator = org.nd4j.codegen.impl.java.DocsGenerator
Imports Nd4jNamespaceGenerator = org.nd4j.codegen.impl.java.Nd4jNamespaceGenerator

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

Namespace org.nd4j.codegen.cli


	''' <summary>
	''' Planned CLI for generating classes
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CLI
	Public Class CLI
		Private Const relativePath As String = "nd4j/nd4j-backends/nd4j-api-parent/nd4j-api/src/main/java/"
		Private Const allProjects As String = "all"
		Private Const sdProject As String = "sd"
		Private Const ndProject As String = "nd4j"

		Public Class ProjectsValidator
			Implements IParameterValidator

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void validate(String name, String value) throws ParameterException
			Public Overrides Sub validate(ByVal name As String, ByVal value As String)
				If name.Equals("-projects") Then
					If Not (value.Equals(allProjects) OrElse value.Equals(ndProject) OrElse value.Equals(sdProject)) Then
						Throw New ParameterException("Wrong projects " & value & "  passed! Must be one of [all, sd, nd4j]")
					End If
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = "-dir", description = "Root directory of deeplearning4j mono repo") private String repoRootDir;
		Private repoRootDir As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = "-docsdir", description = "Root directory for generated docs") private String docsdir;
		Private docsdir As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = "-namespaces", description = "List of namespaces to generate, or 'ALL' to generate all namespaces", required = true) private java.util.List<String> namespaces;
		Private namespaces As IList(Of String)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = "-projects", description = "List of sub-projects - ND4J, SameDiff or both", required = false, validateWith = ProjectsValidator.class) private java.util.List<String> projects = java.util.Collections.singletonList("all");
		Private projects As IList(Of String) = Collections.singletonList("all")

		Friend Enum NS_PROJECT
			ND4J
			SAMEDIFF
		End Enum

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void generateNamespaces(NS_PROJECT project, java.io.File outputDir, String basePackage) throws java.io.IOException
		Private Sub generateNamespaces(ByVal project As NS_PROJECT, ByVal outputDir As File, ByVal basePackage As String)

			Dim usedNamespaces As IList(Of [Namespace]) = New List(Of [Namespace])()

			For Each s As String In namespaces
				If "all".Equals(s, StringComparison.OrdinalIgnoreCase) Then
					Collections.addAll(usedNamespaces, [Namespace].values())
					Exit For
				End If
				Dim ns As [Namespace] = Nothing
				If project = NS_PROJECT.ND4J Then
					ns = [Namespace].fromString(s)
					If ns = Nothing Then
						log.error("Invalid/unknown ND4J namespace provided: " & s)
					Else
						usedNamespaces.Add(ns)
					End If
				Else
					ns = [Namespace].fromString(s)
					If ns = Nothing Then
						log.error("Invalid/unknown SD namespace provided: " & s)
					Else
						usedNamespaces.Add(ns)
					End If
				End If
			Next s

			Dim cnt As Integer = 0
			For i As Integer = 0 To usedNamespaces.Count - 1
				Dim ns As [Namespace] = usedNamespaces(i)
				log.info("Starting generation of namespace: {}", ns)

				Dim javaClassName As String = If(project = NS_PROJECT.ND4J, ns.javaClassName(), ns.javaSameDiffClassName())
				Dim ops As NamespaceOps = ns.getNamespace()

				Dim basePackagePath As String = basePackage.Replace(".", "/") & "/ops/"

				If StringUtils.isNotEmpty(docsdir) Then
					DocsGenerator.generateDocs(i, ops, docsdir, basePackage)
				End If
				If outputDir IsNot Nothing Then
					Dim outputPath As New File(outputDir, basePackagePath & javaClassName & ".java")
					log.info("Output path: {}", outputPath.getAbsolutePath())

					If NS_PROJECT.ND4J = project Then
						Nd4jNamespaceGenerator.generate(ops, Nothing, outputDir, javaClassName, basePackage, docsdir)
					Else
						Nd4jNamespaceGenerator.generate(ops, Nothing, outputDir, javaClassName, basePackage, "org.nd4j.autodiff.samediff.ops.SDOps", docsdir)
					End If
				End If
				cnt += 1
			Next i
			log.info("Complete - generated {} namespaces", cnt)
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void main(String[] args) throws Exception
		Public Shared Sub Main(ByVal args() As String)
			Call (New CLI()).runMain(args)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void runMain(String[] args) throws Exception
		Public Overridable Sub runMain(ByVal args() As String)
			JCommander.newBuilder().addObject(Me).build().parse(args)

			' Either root directory for source code generation or docs directory must be present. If root directory is
			' absenbt - then it's "generate docs only" mode.
			If StringUtils.isEmpty(repoRootDir) AndAlso StringUtils.isEmpty(docsdir) Then
				Throw New System.InvalidOperationException("Provide one or both of arguments : -dir, -docsdir")
			End If
			Dim outputDir As File = Nothing
			If StringUtils.isNotEmpty(repoRootDir) Then
				'First: Check root directory.
				Dim dir As New File(repoRootDir)
				If Not dir.exists() OrElse Not dir.isDirectory() Then
					Throw New System.InvalidOperationException("Provided root directory does not exist (or not a directory): " & dir.getAbsolutePath())
				End If

				outputDir = New File(dir, relativePath)
				If Not outputDir.exists() OrElse Not dir.isDirectory() Then
					Throw New System.InvalidOperationException("Expected output directory does not exist: " & outputDir.getAbsolutePath())
				End If
			End If

			If namespaces Is Nothing OrElse namespaces.Count = 0 Then
				Throw New System.InvalidOperationException("No namespaces were provided")
			End If

			Try
				If projects Is Nothing Then
					projects.Add(allProjects)
				End If
				Dim forAllProjects As Boolean = projects.Count = 0 OrElse projects.Contains(allProjects)
				If forAllProjects OrElse projects.Contains(ndProject) Then
					generateNamespaces(NS_PROJECT.ND4J, outputDir, "org.nd4j.linalg.factory")
				End If
				If forAllProjects OrElse projects.Contains(sdProject) Then
					generateNamespaces(NS_PROJECT.SAMEDIFF, outputDir, "org.nd4j.autodiff.samediff")
				End If
			Catch e As Exception
				log.error(e.ToString())
			End Try
		End Sub
	End Class

End Namespace