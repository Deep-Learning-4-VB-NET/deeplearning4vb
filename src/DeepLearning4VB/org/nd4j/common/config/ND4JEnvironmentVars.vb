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

Namespace org.nd4j.common.config

	Public Class ND4JEnvironmentVars

		''' <summary>
		''' Applicability: nd4j-native, when multiple backends are on classpath<br>
		''' Description: Defines the priority that the CPU/Native backend should be loaded (or attempt to be loaded). If this
		''' is set to a higher value than <seealso cref="BACKEND_PRIORITY_GPU"/> (which has default value 100) the native backend
		''' will be loaded in preference to the CUDA backend, when both are on the classpath. Default value: 0
		''' </summary>
		Public Const BACKEND_PRIORITY_CPU As String = "BACKEND_PRIORITY_CPU"
		''' <summary>
		''' Applicability: nd4j-cuda-xx, when multiple backends are on classpath<br>
		''' Description: Defines the priority that the CUDA (GPU) backend should be loaded (or attempt to be loaded). If this
		''' is set to a higher value than <seealso cref="BACKEND_PRIORITY_CPU"/> (which has default value 0) the GPU backend
		''' will be loaded in preference to the CUDA backend, when both are on the classpath. Default value: 100 - hence
		''' by default, the CUDA backend will be loaded when both it and the CPU/native backend are on the classpath
		''' </summary>
		Public Const BACKEND_PRIORITY_GPU As String = "BACKEND_PRIORITY_GPU"
		''' <summary>
		''' Applicability: always - but only if an ND4J backend cannot be found/loaded via standard ServiceLoader mechanisms<br>
		''' Description: Set this environment variable to a set fully qualified JAR files to attempt to load before failing on
		''' not loading a backend. JAR files should be semi-colon delimited; i.e., "/some/file.jar;/other/path.jar".
		''' This should rarely be required in practice - for example, only in dynamic class loading/dynamic classpath scenarios<br>
		''' For equivalent system property, see <seealso cref="ND4JSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY"/> for the equivalent
		''' system property (that will take precidence if both are set)
		''' </summary>
		Public Const BACKEND_DYNAMIC_LOAD_CLASSPATH As String = "ND4J_DYNAMIC_LOAD_CLASSPATH"
		''' <summary>
		''' Applicability: nd4j-native backend<br>
		''' Description: Sets the number of OpenMP parallel threads for ND4J native operations (and also native BLAS libraries
		''' such as Intel MKL and OpenBLAS).
		''' By default, this will be set to the number of physical cores (i.e., excluding hyperthreading cores), which usually
		''' provides optimal performance. Setting this to a larger value than the number of physical cores (for example, equal
		''' to number of logical cores - i.e., setting to 16 on an 8-core + hypethreading processor) - can result in reduced
		''' performance<br>
		''' Note that if you have a significant number of parallel Java threads (for example, Spark or ParallelWrapper), or
		''' you want to keep some cores free for other programs - you may want to reduce this value.
		''' </summary>
		''' <seealso cref= #ND4J_SKIP_BLAS_THREADS </seealso>
		Public Const OMP_NUM_THREADS As String = "OMP_NUM_THREADS"
		''' <summary>
		''' Applicability: nd4j-native backend<br>
		''' Description: Skips the setting of the <seealso cref="OMP_NUM_THREADS"/> property for ND4J ops. Note that this property
		''' will usually still take effect for native BLAS libraries (MKL, OpenBLAS) even if this property is set
		''' </summary>
		Public Const ND4J_SKIP_BLAS_THREADS As String = "ND4J_SKIP_BLAS_THREADS"
		''' <summary>
		''' Applicability: nd4j-native backend<br>
		''' Description: Whether build-in BLAS matrix multiplication (GEMM) should be used instead of the native BLAS
		''' library such as MKL or OpenBLAS. This can have a noticable performance impact for these ops.
		''' Note that this is typically only useful as a workaround (or test) for bugs in these underlying native libraries,
		''' which are rare (but do occasionally occur on some platforms)
		''' </summary>
		Public Const ND4J_FALLBACK As String = "ND4J_FALLBACK"
		''' <summary>
		''' Applicability: nd4j-parameter-server<br>
		''' Usage: A fallback for determining the local IP the parameter server, if other approaches fail to determine the
		''' local IP
		''' </summary>
		Public Const DL4J_VOID_IP As String = "DL4J_VOID_IP"
		''' <summary>
		''' Applicability: nd4j-cuda-xx<br>
		''' Description:
		''' </summary>
		Public Const ND4J_CUDA_MAX_BLOCK_SIZE As String = "ND4J_CUDA_MAX_BLOCK_SIZE"
		''' <summary>
		''' Applicability: nd4j-cuda-xx<br>
		''' Description:
		''' </summary>
		Public Const ND4J_CUDA_MIN_BLOCK_SIZE As String = "ND4J_CUDA_MIN_BLOCK_SIZE"
		''' <summary>
		''' Applicability: nd4j-cuda-xx<br>
		''' Description:
		''' </summary>
		Public Const ND4J_CUDA_MAX_GRID_SIZE As String = "ND4J_CUDA_MAX_GRID_SIZE"

		''' <summary>
		''' Applicability: nd4j-cuda-xx<br>
		''' Description: This variable defines how many concurrent threads will be able to use same device. Keep in mind, this doesn't affect natural CUDA limitations
		''' </summary>
		Public Const ND4J_CUDA_MAX_CONTEXTS As String = "ND4J_CUDA_MAX_CONTEXTS"

		''' <summary>
		''' Applicability: nd4j-cuda-xx used on multi-GPU systems<br>
		''' Description: If set, only a single GPU will be used by ND4J, even if multiple GPUs are available in the system
		''' </summary>
		Public Const ND4J_CUDA_FORCE_SINGLE_GPU As String = "ND4J_CUDA_FORCE_SINGLE_GPU"
		''' <summary>
		''' Applicability: nd4j-cuda-xx<br>
		''' Description:
		''' </summary>
		Public Const ND4J_CUDA_USE_PREALLOCATION As String = "ND4J_CUDA_USE_PREALLOCATION"
		''' <summary>
		''' Applicability: nd4j-cuda-xx<br>
		''' Description:
		''' </summary>
		Public Const ND4J_CUDA_MAX_DEVICE_CACHE As String = "ND4J_CUDA_MAX_DEVICE_CACHE"
		''' <summary>
		''' Applicability: nd4j-cuda-xx<br>
		''' Description:
		''' </summary>
		Public Const ND4J_CUDA_MAX_HOST_CACHE As String = "ND4J_CUDA_MAX_HOST_CACHE"
		''' <summary>
		''' Applicability: nd4j-cuda-xx<br>
		''' Description:
		''' </summary>
		Public Const ND4J_CUDA_MAX_DEVICE_ALLOCATION As String = "ND4J_CUDA_MAX_DEVICE_ALLOCATION"

		''' <summary>
		''' Applicability: nd4j-native
		''' </summary>
		Public Const ND4J_MKL_FALLBACK As String = "ND4J_MKL_FALLBACK"

		Public Const ND4J_RESOURCES_CACHE_DIR As String = "ND4J_RESOURCES_CACHE_DIR"

		''' <summary>
		''' Applicability: nd4j-native<br>
		''' Description: Set to true to avoid logging AVX warnings (i.e., running generic x86 binaries on an AVX2 system)
		''' </summary>
		Public Const ND4J_IGNORE_AVX As String = "ND4J_IGNORE_AVX"

		''' <summary>
		''' This variable defines how many threads will be used in ThreadPool for parallel execution of linear algebra.
		''' Default value: number of threads supported by this system.
		''' </summary>
		Public Const SD_MAX_THREADS As String = "SD_MAX_THREADS"

		''' <summary>
		''' This variable defines how many threads will be used for any 1 linear algebra operation.
		''' Default value: number of threads supported by this system.
		''' </summary>
		Public Const SD_MASTER_THREADS As String = "SD_MASTER_THREADS"

		''' <summary>
		''' If set, this variable disables use of optimized platform helpers (i.e. mkldnn or cuDNN)
		''' </summary>
		Public Const SD_FORBID_HELPERS As String = "SD_FORBID_HELPERS"

		''' <summary>
		''' If set, this variables defines how much memory application is allowed to use off-heap.
		''' PLEASE NOTE: this option is separate from JVM XMS/XMX options
		''' </summary>
		Public Const SD_MAX_PRIMARY_BYTES As String = "SD_MAX_PRIMARY_BYTES"

		''' <summary>
		''' If set, this variable defines how much memory application is allowed to use ON ALL computational devices COMBINED.
		''' </summary>
		Public Const SD_MAX_SPECIAL_BYTES As String = "SD_MAX_SPECIAL_BYTES"

		''' <summary>
		''' If set, this variable defines how much memory application is allowed to use on any one computational device
		''' </summary>
		Public Const SD_MAX_DEVICE_BYTES As String = "SD_MAX_DEVICE_BYTES"

		Private Sub New()
		End Sub
	End Class

End Namespace