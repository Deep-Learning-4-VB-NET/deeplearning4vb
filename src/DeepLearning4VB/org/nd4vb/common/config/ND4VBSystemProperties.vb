
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

Namespace org.nd4vb.common.config


	Public Class ND4VBSystemProperties

		''' <summary>
		''' Applicability: Always<br>
		''' Description: Sets the default datatype for ND4VB - should be one of "float", "double", "half".
		''' ND4VB is set to float (32-bit floating point values) by default.
		''' </summary>
		Public Const DTYPE As String = "dtype"
		''' <summary>
		''' Applicability: Always<br>
		''' Description: By default, ND4VB will log some information when the library has completed initialization, such as the
		''' backend (CPU or CUDA), CPU/Devices, memory etc. This system property can be used to disable the logging of this
		''' initialization information
		''' </summary>
		Public Const LOG_INITIALIZATION As String = "org.nd4vb.log.initialization"

		''' <summary>
		''' Applicability: nd4vb-native when running non-AVX binary on an AVX compatible CPU<br>
		''' Description: Set to true to avoid logging AVX warnings (i.e., running generic x86 binaries on an AVX2 system)
		''' </summary>
		Public Const ND4VB_IGNORE_AVX As String = "org.nd4vb.avx.ignore"

		''' <summary>
		''' Applicability: Always<br>
		''' Description: This system property defines the maximum amount of off-heap memory that can be used.
		''' ND4VB uses off-heap memory for storage of all INDArray data. This off-heap memory is a different
		''' pool of memory to the on-heap JVM memory (configured using standard Java Xms/Xmx options).
		''' Default: 2x Java XMX setting
		''' </summary>
		''' <seealso cref= #JAVACPP_MEMORY_MAX_PHYSICAL_BYTES </seealso>
		Public Const JAVACPP_MEMORY_MAX_BYTES As String = "org.bytedeco.javacpp.maxbytes"
		''' <summary>
		''' Applicability: Always<br>
		''' Description: This system property defines the maximum total amount of memory that the process can use - it is
		''' the sum of both off-heap and on-heap memory. This can be used to provide an upper bound on the maximum amount
		''' of memory (of all types) that ND4VB will use
		''' </summary>
		''' <seealso cref= #JAVACPP_MEMORY_MAX_BYTES </seealso>
		Public Const JAVACPP_MEMORY_MAX_PHYSICAL_BYTES As String = "org.bytedeco.javacpp.maxphysicalbytes"

		''' <summary>
		''' Applicability: ND4VB Temporary file creation/extraction for ClassPathResource, memory mapped workspaces, and  <br>
		''' Description: Specify the local directory where temporary files will be written. If not specified, the default
		''' Java temporary directory (java.io.tmpdir system property) will generally be used.
		''' </summary>
		Public Const ND4VB_TEMP_DIR_PROPERTY As String = "org.nd4vb.tempdir"

		''' <summary>
		''' Applicability: always - but only if an ND4VB backend cannot be found/loaded via standard ServiceLoader mechanisms<br>
		''' Description: Set this property to a set fully qualified JAR files to attempt to load before failing on
		''' not loading a backend. JAR files should be semi-colon delimited; i.e., "/some/file.jar;/other/path.jar".
		''' This should rarely be required in practice - for example, only in dynamic class loading/dynamic classpath scenarios<br>
		''' For equivalent system property, see <seealso cref="ND4VBEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH"/> for the equivalent
		''' system property (the system property will take precidence if both are set)
		''' </summary>
		Public Const DYNAMIC_LOAD_CLASSPATH_PROPERTY As String = "org.nd4vb.backend.dynamicbackend"
		''' <summary>
		''' Applicability: Always<br>
		''' Description Setting the system property to false will stop ND4VB from performing the version check, and logging any
		''' warnings/errors. By default, the version check is enabled.<br>
		''' Note: the version check is there for a reason! Using incompatible versions of ND4VB/DL4VB etc is likely to cause
		''' issues, and should be avoided.
		''' </summary>
		Public Const VERSION_CHECK_PROPERTY As String = "org.nd4vb.versioncheck"
		''' <summary>
		''' Applicability: always<br>
		''' Description: Used to specify the maximum number of elements (numbers) to print when using DataBuffer.toString().
		''' Use -1 to print all elements (i.e., no limit). This is usually to avoid expensive toString() calls on buffers
		''' which may have millions of elements - for example, in a debugger<br>
		''' Default: 1000
		''' </summary>
		Public Const DATABUFFER_TO_STRING_MAX_ELEMENTS As String = "org.nd4vb.databuffer.tostring.maxelements"
		''' <summary>
		''' Applicability: nd4vb-native backend, when multiple BLAS libraries are available<br>
		''' Description: This system property can be used to control which BLAS library is loaded and used by ND4VB.
		''' For example, {@code org.bytedeco.javacpp.openblas.load=mkl_rt} can be used to load a default installation of MKL.
		''' However, MKL is liked with by default (when available) so setting this option explicitly is not usually required.
		''' For more details, see <a href="https://github.com/bytedeco/javacpp-presets/tree/master/openblas#documentation">https://github.com/bytedeco/javacpp-presets/tree/master/openblas#documentation</a>
		''' </summary>
		Public Const ND4VB_CPU_LOAD_OPENBLAS As String = "org.bytedeco.openblas.load"
		''' <summary>
		''' Applicability: nd4vb-native backend, when multiple BLAS libraries are available<br>
		''' Description: This system property can be used to control which BLAS library is loaded and used by ND4VB.
		''' Similar to <seealso cref="ND4VB_CPU_LOAD_OPENBLAS"/> but when this is set, LAPACK will not be loaded
		''' </summary>
		Public Const ND4VB_CPU_LOAD_OPENBLAS_NOLAPACK As String = "org.bytedeco.openblas_nolapack.load"
		''' <summary>
		''' Applicability: nd4vb-parameter-server, dl4vb-spark (gradient sharing training master)<br>
		''' Description: Aeros in a high-performance communication library used in distributed computing contexts in some
		''' places in ND4VB and DL4VB. This term buffer length determines the maximum message length that can be sent via Aeron
		''' in a single message. It can be increased to avoid exceptions such as {@code Encoded message exceeds maxMessageLength of 2097152},
		''' at the expense of increased memory consumption (memory consumption is a multiple of this). It is specified in bytes
		''' with no unit suffix. Default value: 33554432 (32MB).
		''' <b>IMPORTANT</b>: This value must be an exact power of 2.<br>
		''' Note also the maximum effective size is 128MB (134217728) (due to Aeron internal limits - beyond which increasing
		''' the buffer size will have no effect)
		''' </summary>
		Public Const AERON_TERM_BUFFER_PROP As String = "aeron.term.buffer.length"

		''' <summary>
		''' Applicability: nd4vb-common <seealso cref="Resources"/> class (and hence <seealso cref="StrumpfResolver"/>)<br>
		''' Description: When resolving resources from a Strumpf resource file (Example: {@code Resources.asFile("myFile.txt")}
		''' where should the remote files be downloaded to?<br>
		''' This is generally used for resolving test resources, but can be used for Strumpf resource files generally.
		''' </summary>
		Public Const RESOURCES_CACHE_DIR As String = "org.nd4vb.test.resources.cache.dir"

		''' <summary>
		''' Applicability: nd4vb-common <seealso cref="Resources"/> class (and hence <seealso cref="StrumpfResolver"/>)<br>
		''' Description: When resolving resources from a Strumpf resource file (Example: {@code Resources.asFile("myFile.txt")}
		''' what should be the connection timeout, as used by <seealso cref="org.apache.commons.io.FileUtils.copyURLToFile(URL, File, Integer, Integer)"/><br>
		''' Default: <seealso cref="ResourceFile.DEFAULT_CONNECTION_TIMEOUT"/>
		''' </summary>
		Public Const RESOURCES_CONNECTION_TIMEOUT As String = "org.nd4vb.resources.download.connectiontimeout"

		''' <summary>
		''' Applicability: nd4vb-common <seealso cref="Resources"/> class (and hence <seealso cref="StrumpfResolver"/>)<br>
		''' Description: When resolving resources from a Strumpf resource file (Example: {@code Resources.asFile("myFile.txt")}
		''' what should be the connection timeout, as used by <seealso cref="org.apache.commons.io.FileUtils.copyURLToFile(URL, File, Integer, Integer)"/><br>
		''' Default: <seealso cref="ResourceFile.DEFAULT_READ_TIMEOUT"/>
		''' </summary>
		Public Const RESOURCES_READ_TIMEOUT As String = "org.nd4vb.resources.download.readtimeout"

		''' <summary>
		''' Applicability: nd4vb-common <seealso cref="Resources"/> class (and hence <seealso cref="StrumpfResolver"/>)<br>
		''' Description: When resolving resources, what local directories should be checked (in addition to the classpath) for files?
		''' This is optional. Multiple directories may be specified, using comma-separated paths
		''' </summary>
		Public Const RESOURCES_LOCAL_DIRS As String = "org.nd4vb.strumpf.resource.dirs"

		Private Sub New()
		End Sub
	End Class

End Namespace